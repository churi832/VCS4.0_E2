
#ifndef LIBSOSLAB_USER_DEFINES_H
#define LIBSOSLAB_USER_DEFINES_H

//////////////////
/* Header Files */
//////////////////
#include <iostream>
#include <vector>

/////////////
/* Defines */
/////////////
typedef enum
{
	FW_BOOT = 1,
	FW_APP,
} FW_FILE_TYPE;

namespace SOSLAB
{
	typedef struct _ETHERNETINFO
	{
		std::string sensor_ip;
		int sensor_port;
		std::string pc_ip;
		int pc_port;
		std::string subnet_mask;
		std::string gateway_addr;
	} ethernet_info_t;

	typedef struct _FRAMEDATA
	{
		std::vector<double> distance;
		std::vector<double> pulse_width;
		std::vector<double> angle;
		std::vector<double> x;
		std::vector<double> y;
		uint8_t input_area;
		uint8_t output_level;
		uint8_t error_bit;
		uint16_t dist_offset;
		uint16_t backreflector_pulse_width;
		uint16_t pd_hv;
		uint16_t ld_hv;
		uint16_t pd_temp;
		uint16_t ld_temp;
	} framedata_t;

} // namespace SOSLAB

#endif // LIBSOSLAB_USER_DEFINES_H
