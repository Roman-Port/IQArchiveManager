project ("IQArchiveManager")

# Add VOLK
set(VOLK_PYTHON_INSTALL_FILES FALSE)
set(VOLK_PYTHON_INSTALL_PROGRAMS FALSE)
add_subdirectory(volk)

# Create native lib
add_library(iqam_native SHARED  "api.cpp" "api.h" "pre.h" "pre.cpp"  )

# Link VOLK
target_link_libraries(iqam_native volk)

# Set compiler options
if (MSVC)
    target_compile_options(iqam_native PRIVATE /O2 /Ob2 /std:c++17 /EHsc) # /O2 /Ob2
elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    target_compile_options(iqam_native PRIVATE -O3 -std=c++17)
else ()
    target_compile_options(iqam_native PRIVATE -O3 -std=c++17)
endif ()

# Add SDR++ DSP
target_include_directories(iqam_native PRIVATE dsp/lib)