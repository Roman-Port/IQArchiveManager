#include "api.h"

#include <mutex>

void iqam_version(int* major, int* minor) {
	*major = 0;
	*minor = 1;
}

void* iqam_malloc(size_t size) {
	return volk_malloc(size, volk_get_alignment());
}

void iqam_free(void* buffer) {
	volk_free(buffer);
}

//

iqam_pre* iqam_pre_create(int bufferSize) {
	return new iqam_pre(bufferSize);
}

void iqam_pre_destroy(iqam_pre* ctx) {
	delete ctx;
}

double iqam_pre_get_property(iqam_pre* ctx, int index) {
	return ctx->get_property(index);
}

void iqam_pre_set_property(iqam_pre* ctx, int index, double value) {
	ctx->set_property(index, value);
}

void iqam_pre_init(iqam_pre* ctx) {
	ctx->init();
}

void iqam_pre_process(iqam_pre* ctx, const dsp::complex_t* input, uint8_t* audioOut, int* audioOutCount, uint8_t* rdsOut, int* rdsOutCount, int count) {
	ctx->process(input, audioOut, audioOutCount, rdsOut, rdsOutCount, count);
}