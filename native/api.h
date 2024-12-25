#pragma once

#include <mutex>
#include "pre.h"

#define EXPORT_API extern "C" 

//utility
EXPORT_API void iqam_version(int* major, int* minor);
EXPORT_API void* iqam_malloc(size_t size);
EXPORT_API void iqam_free(void* buffer);

//pre
EXPORT_API iqam_pre* iqam_pre_create(int bufferSize);
EXPORT_API void iqam_pre_destroy(iqam_pre* ctx);
EXPORT_API double iqam_pre_get_property(iqam_pre* ctx, int index);
EXPORT_API void iqam_pre_set_property(iqam_pre* ctx, int index, double value);
EXPORT_API void iqam_pre_init(iqam_pre* ctx);
EXPORT_API void iqam_pre_process(iqam_pre* ctx, const dsp::complex_t* input, uint8_t* audioOut, int* audioOutCount, uint8_t* rdsOut, int* rdsOutCount, int count);