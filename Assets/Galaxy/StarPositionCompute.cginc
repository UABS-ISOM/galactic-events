// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

float _Age;

float4 _EllipseSize;

float3 ComputeStarPosition(StarVertDescriptor star)
{
	float curveOffset = star.curveOffset + _Age;
	float ellipseOffset = star.ellipseOffset;
	float ellipseDistance = star.ellipseDistance;

	float xRadii = _EllipseSize.x;
	float zRadii = _EllipseSize.y;

	float ellipseScale = ellipseDistance;

	float x = cos(curveOffset) * xRadii;
	float z = sin(curveOffset) * zRadii;

	float zp = z * cos(ellipseOffset) - x * sin(ellipseOffset);
	float xp = z * sin(ellipseOffset) + x * cos(ellipseOffset);

	x = xp;
	z = zp;

	return float3(x * ellipseScale, star.yOffset, z * ellipseScale);
}