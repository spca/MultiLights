#pragma once
#include"Object.h"
enum LightType {
	Directional,
	POINT,
	SPOT

};
class Light : public Object {
public:
	Light(glm::vec3 pos, float* vertics, int32_t count, LightType type,glm::vec3 dir) :Object(pos, vertics, count),type(type),direction(dir) {
	}
private:
	LightType type;
	glm::vec3 direction;

};