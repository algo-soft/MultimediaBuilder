#version 330 core

layout (location = 0) in vec3 aPos; // Position attribute
layout (location = 1) in vec2 aTexCoord; // Texture coordinate attribute

out vec2 TexCoord;

uniform mat4 model;
uniform mat4 projection;

void main()
{
    gl_Position = projection * model * vec4(aPos, 1.0);
    TexCoord = aTexCoord; // Apply texture transformation
}
