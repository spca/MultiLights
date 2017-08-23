#pragma once
#include <glm/glm.hpp>
class Object {
public:
	Object(glm::vec3 pos,float* vertics,int32_t count):worldPos(pos),vertics(vertics),vertexCount(count) {
	}
	Object(float x, float y, float z, float* vertics, int32_t count):worldPos(glm::vec3(x,y,z)), vertics(vertics), vertexCount(count) {
	}
private:
	glm::vec3 worldPos;
	int32_t vertexCount;
	float* vertics;
	
};
