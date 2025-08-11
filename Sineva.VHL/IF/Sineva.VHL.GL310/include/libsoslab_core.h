
#ifndef LIBSOSLAB_CORE_H
#define LIBSOSLAB_CORE_H

///////////////////////////////
/* Macros for DLL generation */
///////////////////////////////
#ifndef SOSLAB_EXPORTS
#ifdef LIBSOSLABCORE_EXPORTS
#if defined(_MSC_VER)
#define SOSLAB_EXPORTS __declspec(dllexport)
#elif defined(__GNUC__)
#define SOSLAB_EXPORTS __attribute__((visibility("default")))
#endif
#else
#if defined(_MSC_VER)
#define SOSLAB_EXPORTS __declspec(dllimport)
#else
#define SOSLAB_EXPORTS
#endif
#endif
#endif

#if defined(_MSC_VER)
#pragma warning(disable : 4702) // warning C4702: unreachable code
#pragma warning(disable : 4100) // warning C4100 : unreferenced formal parameter
#pragma warning(disable : 4189) // warning C4189 : local variable is initialized but not referenced
#endif

//////////////////
/* Header Files */
//////////////////
#include <iostream>
#include <vector>
#include <stdio.h>

#include "libsoslab_core_defines.h"

///////////
/* Class */
///////////
namespace SOSLAB
{
	class SOSLAB_EXPORTS CORE
	{
	public:
		CORE();
		~CORE();

		bool connectUDP(const std::string &udp_sensor_ip = "10.110.1.2", const int udp_sensor_port = 2000, const std::string &udp_pc_ip = "10.110.1.3", const int udp_pc_port = 3000);
		bool connectSerial(const std::string &port_name, const int baudrate = 921600);
		void disconnect();
		bool isConnected();
		bool writePacket(const SOSLAB::payload_info_t payload_info, std::vector<uint8_t> &ret_data, const double timeout = 5.0);
		bool readStream(const uint8_t sm, const uint8_t cat0, const uint8_t cat1, std::vector<uint8_t> &ret_data, const double timeout = 5.0);
		bool isSerialConnected();

	private:
		void *core_;

	}; // class CORE

} // namespace SOSLAB

///////////////////////
/* C-style Functions */
///////////////////////
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	SOSLAB_EXPORTS void *GL3_CORE_createInstance();
	SOSLAB_EXPORTS void GL3_CORE_releaseInstance(void *core);

	SOSLAB_EXPORTS bool GL3_CORE_connectUDP(void *core, const char *udp_sensor_ip, const int udp_sensor_port, const char *udp_pc_ip, const int udp_pc_port);
	SOSLAB_EXPORTS bool GL3_CORE_connectSerial(void *core, const char *port_name, const int baudrate);
	SOSLAB_EXPORTS void GL3_CORE_disconnect(void *core);
	SOSLAB_EXPORTS bool GL3_CORE_isConnected(void *core);

	SOSLAB_EXPORTS bool GL3_CORE_writePacket(void *core, const uint8_t sm, const uint8_t bi, const uint8_t cat0, const uint8_t cat1, const uint8_t *dtn, const size_t dtn_size, uint8_t *ret_data, size_t *ret_data_size, const double timeout);
	SOSLAB_EXPORTS bool GL3_CORE_readStream(void *core, const uint8_t sm, const uint8_t cat0, const uint8_t cat1, uint8_t *ret_data, size_t *ret_data_size, const double timeout);
	SOSLAB_EXPORTS bool GL3_CORE_isSerialConnected(void *core);

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // LIBSOSLAB_CORE_H
