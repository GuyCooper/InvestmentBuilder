#include "stdafx.h"

namespace
{
	//General logger class
	class Logger
	{
	public:
		void LogMessage(std::string message)
		{
			std::cout << message << std::endl;
		}
	};

	Logger _logger;
}

namespace process_manager_utils
{
	//Method returns the filename of the calling process
	std::string getProcessFilename()
	{
		char szPath[MAX_PATH];

		if (!GetModuleFileName(NULL, szPath, MAX_PATH))
		{
			throw new std::runtime_error("Unable to get module filename");
		}

		return std::string(szPath);
	}

	//Method returns the path of the current process.
	std::string getProcessPath()
	{
		std::string processname = getProcessFilename();
		std::size_t index = processname.find_last_of('\\');
		return processname.substr(0, index);
	}

	void logMessage(std::string message)
	{
		_logger.LogMessage(message);
	}

	// Methods returns the last system error
	std::string getLastErrorString()
	{
		LPVOID lpMsgBuf;
		DWORD dw = GetLastError();

		FormatMessage(
			FORMAT_MESSAGE_ALLOCATE_BUFFER |
			FORMAT_MESSAGE_FROM_SYSTEM |
			FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,
			dw,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			(LPTSTR)&lpMsgBuf,
			0, NULL);

		std::string strError = (LPCTSTR)lpMsgBuf;
		LocalFree(lpMsgBuf);

		return strError;
	}
}