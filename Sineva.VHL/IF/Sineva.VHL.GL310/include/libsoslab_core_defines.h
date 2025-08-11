
#ifndef LIBSOSLAB_CORE_DEFINES_H
#define LIBSOSLAB_CORE_DEFINES_H

//////////////////
/* Header Files */
//////////////////
#include <iostream>

/////////////
/* Defines */
/////////////
typedef enum
{
	SM_SET = 0,
	SM_GET,
	SM_STREAM,
	SM_ERROR = 255,
} PAYLOAD_SM;

typedef enum
{
	BI_PC2DEV = 0x21,
	BI_DEV2PC = 0x12,
} PAYLOAD_BI;

namespace SOSLAB
{
	typedef struct _PAYLOAD_INFO
	{
		uint8_t sm;
		uint8_t bi;
		uint8_t cat0;
		uint8_t cat1;
		std::vector<uint8_t> dtn;
	} payload_info_t;
} // namespace SOSLAB

#endif // LIBSOSLAB_CORE_DEFINES_H
