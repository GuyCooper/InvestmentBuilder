#include "stdafx.h"
#include <list>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp>
#include <boost/foreach.hpp>
#include <boost/shared_ptr.hpp>
#include <sstream>
#include <stdio.h>      /* printf, scanf */
#include <time.h>       /* time_t, struct tm, time, mktime */
#include "Utils.h"

namespace
{
	// ProcessItem structure. Defines the  comfiguration details for a single process.
	struct ProcessItem
	{
		std::string m_name;
		std::string m_path;
		std::string m_folder;
		int m_instance;
		time_t m_nextRunTime;
		HANDLE m_hProcess;		
	};

	// Spawn a process.
	HANDLE SpawnProcess(std::string path, std::string folder)
	{
		process_manager_utils::logMessage("running process: " + path);

		PROCESS_INFORMATION pi;
		STARTUPINFO si;

		ZeroMemory(&pi, sizeof(PROCESS_INFORMATION));
		ZeroMemory(&si, sizeof(STARTUPINFO));

		size_t count = path.size() + 1;
		char *szCommandLine = (char*)calloc(count, sizeof(char));
		strncpy_s(szCommandLine, count, path.c_str(), path.size());
		
		si.cb = sizeof(STARTUPINFO);

		bool ok = false;
		BOOL success = CreateProcess(NULL,
			szCommandLine,
			NULL,
			NULL,
			false,
			CREATE_NEW_CONSOLE,
			NULL,
			folder.c_str(),
			&si,
			&pi);

		if (success != FALSE)
		{
			ok = true;
			//ok = ::WaitForInputIdle(pi.hProcess, INFINITE) == 0;

			CloseHandle(pi.hThread);
		}

		if (ok)
		{
			process_manager_utils::logMessage("process started ok");
		}
		else
		{
			process_manager_utils::logMessage("process failed to start: Error: " + process_manager_utils::getLastErrorString());
		}

		free(szCommandLine);

		return pi.hProcess;
	}

	// Kill ap process.
	void KillProcess(HANDLE hProcess)
	{
		::TerminateProcess(hProcess, 0);
	}

	// ProcessManager class. Manage the confgured processes.
	class ProcessManager
	{
	public:
		void StartProcesses(std::string configurationfile)
		{
			process_manager_utils::logMessage("starting processes..");
			//std::string filename = "processes.json";
			// Create empty property tree object
			
			boost::property_tree::ptree tree;
	
			// Parse the XML into the property tree.
			boost::property_tree::read_xml(configurationfile, tree);

			BOOST_FOREACH(boost::property_tree::ptree::value_type &process, tree.get_child("configuration.processes"))
			{
				// The data function is used to access the data stored in a node.
				std::string processName = process.second.get<std::string>("name");
				std::string processPath = process.second.get<std::string>("path");
				std::string workingFolder = process.second.get<std::string>("workingpath");
				int instances = process.second.get<int>("instances", 1);
				std::string runTime = process.second.get<std::string>("runtime", "");

				for (int instance = 0; instance < instances; ++instance)
				{
					//create process..
					std::ostringstream ss;
					ss << "Starting Process " << processName << std::endl << "Instance " << instance << std::endl <<
						"Path " << processPath << std::endl << "Working Folder" << workingFolder << std::endl;

					process_manager_utils::logMessage(ss.str());

					m_processes.push_back(ProcessItem{processName, processPath, workingFolder, instance, 0, NULL});
					
					ProcessItem& processItem = m_processes.back();

					// Set the next runtime field for this process (if it has one)
					SetNextRuntimeField(runTime, processItem);

					if (processItem.m_nextRunTime == 0)
					{
						// process is just a one shot process. do it here.
						processItem.m_hProcess = SpawnProcess(processItem.m_path, processItem.m_folder);
						::Sleep(1000);
					}
				}
			}
			// Use the throwing version of get to find the debug filename.
		}

		// Parse the runtime string.
		bool gettimeparts(std::string strtime, int& hour, int& minute, int&second)
		{
			char *szTime = (char*)strtime.c_str();
			char *pch = strtok(szTime, ":");
			if (pch == NULL) return false;

			hour = atoi(pch);
			pch = strtok(NULL, ":");
			if (pch == NULL)
			{
				minute = 0;
				second = 0;
				return true;
			}
			
			minute = atoi(pch);

			pch = strtok(NULL, ":");
			second = pch != NULL ? atoi(pch) : 0;
			
			return true;
		}

		// Convert the runtime string to a time_t for the specified process item.
		void SetNextRuntimeField(std::string strtime, ProcessItem& processItem)
		{
			int hour, minute, second;
			if (gettimeparts(strtime, hour, minute, second))
			{
				time_t utcnow = time(NULL);
				struct tm nextruntime = *::gmtime(&utcnow);
				nextruntime.tm_hour = hour;
				nextruntime.tm_min = minute;
				nextruntime.tm_sec = second;

				process_manager_utils::logMessage("nextruntime today: " + std::string(::asctime(&nextruntime)));
				processItem.m_nextRunTime = ::mktime(&nextruntime);
			}
		}

		// Increment the nextruntime field for the specified process item.
		void IncrementNextRuntimeField(ProcessItem& processItem)
		{
			processItem.m_nextRunTime = processItem.m_nextRunTime + (60 * 60 * 24);
			struct tm tmNext = *::gmtime(&processItem.m_nextRunTime);
			process_manager_utils::logMessage("nextruntime: " + std::string(::asctime(&tmNext)));
		}

		// Stop all running processes.
		void StopProcesses()
		{
			process_manager_utils::logMessage("stopping processes..");
			BOOST_FOREACH(ProcessItem& item, m_processes)
			{
				StopProcess(item);
			}
		}

		//Stop a single process
		void StopProcess(ProcessItem& processItem)
		{
			if (processItem.m_hProcess == NULL)
			{
				// Process not running anyway
				return;
			}

			if(WaitForSingleObject(processItem.m_hProcess,0) == WAIT_TIMEOUT)
			{
				// Process handle not signalled so process is still running. kill it.
				KillProcess(processItem.m_hProcess);
				WaitForSingleObject(processItem.m_hProcess, 3000);
				::CloseHandle(processItem.m_hProcess);
				processItem.m_hProcess = NULL;
			}
		}

		// Check the status of running processes and start any that need to be started.
		void CheckProcesses()
		{
			time_t utcnow = time(NULL);
			BOOST_FOREACH(ProcessItem& processItem, m_processes)
			{
				ScheduleProcess(processItem, utcnow);
			}
		}

		// Check the next runtime for this process. if it has passed then spawn the process and increment
		// the nextruntime
		void ScheduleProcess(ProcessItem& processItem, time_t utcnow)
		{
			if (processItem.m_nextRunTime > 0)
			{
				if (processItem.m_nextRunTime <= utcnow)
				{
					IncrementNextRuntimeField(processItem);
					processItem.m_hProcess = SpawnProcess(processItem.m_path, processItem.m_folder);
				}
			}
		}

	private:
		std::list<ProcessItem> m_processes;
	};

	ProcessManager _manager;
}

namespace process_manager
{
	void StartProcesses(std::string configurationFile)
	{
		_manager.StartProcesses(configurationFile);
	}

	// SHutdown all processes.
	void ShutdownProcesses()
	{
		_manager.StopProcesses();
	}

	// Check the process for scheduled runt time.
	void CheckProcesses()
	{
		_manager.CheckProcesses();
	}

	// Return the name of this service as specified in the configuration file (optional).
	std::string GetServiceName(std::string configurationFile)
	{
		boost::property_tree::ptree tree;
		// Parse the XML into the property tree.
		boost::property_tree::read_xml(configurationFile, tree);
		return tree.get<std::string>("configuration.servicename");
	}
}


