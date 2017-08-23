#version 330 core
struct DirLight{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight{
    vec3 position;
    float constant;
    float linear;
    float quadratic;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight{
    vec3 position;
    vec3 direction;
    float cutoff;
    float outcutoff;
     float constant;
    float linear;
    float quadratic;   
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
}; 
vec3 CalcDirLight(DirLight light,vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal , vec3 fragPos, vec3 viewDir);
#define NR_POINT_LIGHTS 4
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform DirLight dirLight;
uniform SpotLight spotLight;
uniform vec3 viewPos;
uniform Material material;
in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;
out vec4 outFragColor;
void main(){

    // 属性
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // 第一阶段：定向光照
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    // 第二阶段：点光源
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);    
    // 第三阶段：聚光
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir);    

    outFragColor = vec4(result, 1.0);

}


vec3 CalcDirLight(DirLight light,vec3 normal, vec3 viewDir)
{
    vec3 lightdir = normalize(-light.direction);
    vec3 norm = normalize(normal);
    float diff =max(dot(normal,lightdir),0.0);
    vec3 reflectdir = reflect(-lightdir,normal);
    float spec = pow(max(dot(viewDir,reflectdir),0.0),material.shininess);
    vec3 ambient = light.ambient * vec3(texture(material.diffuse,TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse,TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular,TexCoords));
    return (ambient + diffuse + specular);
}



vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir){
    vec3 lightdir = normalize(light.position - fragPos);
    float diff = max(dot(lightdir,normal),0.0);
    vec3 reflectdir  = reflect(-lightdir,normal);
    float spec = pow(max(dot(viewDir,reflectdir),0.0), material.shininess);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0/(light.constant + light.linear * distance + light.quadratic * distance * distance);
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse+ specular);
}

vec3 CalcSpotLight(SpotLight light,vec3 normal,vec3 fragPos,vec3 viewDir)
{
    vec3 lightdir = normalize(light.position - fragPos);
    float diff = max(dot(normal,lightdir),0.0);
    vec3 reflectDir = reflect(-lightdir,normal);
    float spec = pow(max(dot(viewDir,reflectDir),0.0), material.shininess);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0/(light.constant + light.linear * distance + light.quadratic * distance * distance);
    float theta = dot(lightdir,normalize(-light.direction));
    float epsilon = light.cutoff - light.outcutoff;
    float intensity = clamp((theta- light.cutoff)/epsilon,0.0,1.0);
   // combine results
   
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);

}