#include "stdafx.h"
#include <list>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp>
#include <boost/foreach.hpp>
#include <boost/shared_ptr.hpp>
#include <sstream>

#include "Utils.h"

namespace
{
	struct ProcessItem
	{
		std::string m_name;
		std::string m_path;
		std::string m_folder;
		int m_instance;
		HANDLE m_hProcess;		
	};

	HANDLE SpawnProcess(std::string path, std::string folder)
	{
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

	void KillProcess(HANDLE hProcess)
	{
		::TerminateProcess(hProcess, 0);
	}

	//boost::shared_ptr<ProcessItem> ProcessItemPtr;

	class ProcessManager
	{
	public:
		void StartProcesses(std::string configurationfile)
		{
			process_manager_utils::logMessage("starting processes..");
			//std::string filename = "processes.json";
			// Create empty property tree object
			
			boost::property_tree::ptree tree;
	
			// Parse the JSON into the property tree.
			boost::property_tree::read_xml(configurationfile, tree);

			BOOST_FOREACH(boost::property_tree::ptree::value_type &process, tree.get_child("configuration.processes"))
			{
				// The data function is used to access the data stored in a node.
				std::string processName = process.second.get<std::string>("name");
				std::string processPath = process.second.get<std::string>("path");
				std::string workingFolder = process.second.get<std::string>("workingpath");
				int instances = process.second.get<int>("instances");

				for (int instance = 0; instance < instances; ++instance)
				{
					//create process..
					std::ostringstream ss;
					ss << "Starting Process " << processName << std::endl << "Instance " << instance << std::endl <<
						"Path " << processPath << std::endl << "Working Folder" << workingFolder << std::endl;

					process_manager_utils::logMessage(ss.str());

					m_processes.push_back(ProcessItem{processName, processPath, workingFolder, instance, NULL});
					
					ProcessItem& processItem = m_processes.back();

					processItem.m_hProcess = SpawnProcess(processItem.m_path, processItem.m_folder);
					::Sleep(1000);
				}
			}


			// Use the throwing version of get to find the debug filename.
		}

		void StopProcesses()
		{
			process_manager_utils::logMessage("stopping processes..");
			BOOST_FOREACH(ProcessItem& item, m_processes)
			{
				if(item.m_hProcess != NULL)
				{
					KillProcess(item.m_hProcess);
					::CloseHandle(item.m_hProcess);
					item.m_hProcess = NULL;
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

	void ShutdownProcesses()
	{
		_manager.StopProcesses();
	}

	std::string GetServiceName(std::string configurationFile)
	{
		boost::property_tree::ptree tree;
		// Parse the XML into the property tree.
		boost::property_tree::read_xml(configurationFile, tree);
		return tree.get<std::string>("configuration.servicename");
	}
}


