cmake_minimum_required(VERSION 3.2)

include("cmake/HunterGate.cmake")
HunterGate(
    URL "https://github.com/ruslo/hunter/archive/v0.16.15.tar.gz"
    SHA1 "6974c2150fc0d3b09de3ad1efcbf15d360647ffa"
)
project(NebulaCLI VERSION 0.1.0)

include(CTest)
enable_testing()

hunter_add_package(CLI11)
find_package(CLI11 CONFIG REQUIRED)

add_executable(NebulaCLI main.cpp)
target_link_libraries(NebulaCLI CLI11::CLI11)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
