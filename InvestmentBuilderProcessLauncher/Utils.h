#pragma once

namespace process_manager_utils 
{
	std::string getProcessPath();
	std::string getProcessFilename();
	void logMessage(std::string message);
	std::string getLastErrorString();
}
