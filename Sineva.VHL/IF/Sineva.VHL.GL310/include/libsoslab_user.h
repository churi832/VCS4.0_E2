
#ifndef LIBSOSLAB_USER_H
#define LIBSOSLAB_USER_H

///////////////////////////////
/* Macros for DLL generation */
///////////////////////////////
#ifndef SOSLAB_EXPORTS
#ifdef LIBSOSLABUSER_EXPORTS
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
#include <memory>

#include "libsoslab_core.h"
#include "libsoslab_user_defines.h"

///////////
/* Class */
///////////
namespace SOSLAB
{
	class SOSLAB_EXPORTS USER
	{
	public:
		USER();
		~USER();

		bool getEthernetInfo(std::unique_ptr<SOSLAB::CORE> &core, SOSLAB::ethernet_info_t &ethernet_info, std::string &mac_addr);
		bool setEthernetInfo(std::unique_ptr<SOSLAB::CORE> &core, const SOSLAB::ethernet_info_t &ethernet_info);
		bool getSerialNum(std::unique_ptr<SOSLAB::CORE> &core, std::string &serialnum);
		bool getFWVersion(std::unique_ptr<SOSLAB::CORE> &core, std::vector<uint8_t> &fw_version);
		bool getStreamEnable(std::unique_ptr<SOSLAB::CORE> &core, bool &enable);
		bool setStreamEnable(std::unique_ptr<SOSLAB::CORE> &core, const bool enable);
		bool getLidarData(std::unique_ptr<SOSLAB::CORE> &core, SOSLAB::framedata_t &frame_data, const bool filter_on = true);

		bool doFWUpload(std::unique_ptr<SOSLAB::CORE> &core, const std::string &boot_file_path, const std::string &app_file_path);

	private:
		void *user_;

	}; // class USER

} // namespace SOSLAB

///////////////////////
/* C-style Functions */
///////////////////////
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	SOSLAB_EXPORTS void *GL3_USER_createInstance();
	SOSLAB_EXPORTS void GL3_USER_releaseInstance(void *user);

	SOSLAB_EXPORTS bool GL3_USER_getEthernetInfo(void *core, void *user, char *sensor_ip, size_t *sensor_ip_size, int *sensor_port, char *pc_ip, size_t *pc_ip_size, int *pc_port, char *subnet_mask, size_t *subnet_mask_size, char *gateway_addr, size_t *gateway_addr_size, char *mac_addr, size_t *mac_addr_size);
	SOSLAB_EXPORTS bool GL3_USER_setEthernetInfo(void *core, void *user, const char *sensor_ip, const size_t sensor_ip_size, const int sensor_port, const char *pc_ip, const size_t pc_ip_size, const int pc_port, const char *subnet_mask, const size_t subnet_mask_size, const char *gateway_addr, const size_t gateway_addr_size);
	SOSLAB_EXPORTS bool GL3_USER_getSerialNum(void *core, void *user, char *serialnum, size_t *serialnum_size);
	SOSLAB_EXPORTS bool GL3_USER_getFWVersion(void *core, void *user, uint8_t *fw_version, size_t *fw_version_size);
	SOSLAB_EXPORTS bool GL3_USER_getStreamEnable(void *core, bool *enable);
	SOSLAB_EXPORTS bool GL3_USER_setStreamEnable(void *core, void *user, const bool enable);
	SOSLAB_EXPORTS bool GL3_USER_getLidarData(void *core, void *user, size_t *data_size, double *distance, double *pulse_width, double *angle, double *x, double *y, uint8_t *input_area, uint8_t *output_level, uint8_t *error_bit, uint16_t *dist_offset, uint16_t *backreflector_pulse_width, uint16_t *pd_hv, uint16_t *ld_hv, uint16_t *pd_temp, uint16_t *ld_temp, const bool filter_on);

	SOSLAB_EXPORTS bool GL3_USER_doFWUpload(void *core, void *user, const char *boot_file_path, const char *app_file_path);

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // LIBSOSLAB_USER_H
