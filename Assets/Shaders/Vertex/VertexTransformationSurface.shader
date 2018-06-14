Shader "Surface/Vertex" {
    Properties {
		// === TWIST ===
		[Header(Twist)]
		// _TwistAngle: Angle of rotation at the extremes (in degrees)
		_TwistAngleX("Twist Angle X", Range(-360,360)) = 0
		_TwistAngleY("Twist Angle Y", Range(-360,360)) = 0
		_TwistAngleZ("Twist Angle Z", Range(-360,360)) = 0

		[Space(10)]

		// === STRETCH ===
		[Header(Stretch)]
		// _StretchAmount: How much to stretch along main axis
		// _StretchStrength: How much to exagerate stretch
		_StretchAmountX("Stretch Amount X", Range(-2,2)) = 0
		_StretchStrengthX("Stretch Strength X", Range(0,3)) = 1
		[Space(5)]
		_StretchAmountY("Stretch Amount Y", Range(-2,2)) = 0
		_StretchStrengthY("Stretch Strength Y", Range(0,3)) = 1
		[Space(5)]
		_StretchAmountZ("Stretch Amount Z", Range(-2,2)) = 0
		_StretchStrengthZ("Stretch Strength Z", Range(0,3)) = 1

		[Space(10)]
		// === BEND ===
		[Header(Bend X)]
		// Params for bending region, expressed in percentage
		_XMin("Min value X", Range(0,1)) = 0
		_XMax("Max value X", Range(0,1)) = 1
		_X0("Starting value X", Range(0,1)) = 0
		_BendRateX("Bend Rate X", Float) = 0
		[Space(5)]
		[Header(Bend Y)]
		_YMin("Min value Y", Range(0,1)) = 0
		_YMax("Max value Y", Range(0,1)) = 1
		_Y0("Starting value Y", Range(0,1)) = 0
		_BendRateY("Bend Rate Y", Float) = 0
		[Space(5)]
		[Header(Bend Z)]
		_ZMin("Min value Z", Range(0,1)) = 0
		_ZMax("Max value Z", Range(0,1)) = 1
		_Z0("Starting value Z", Range(0,1)) = 0
		_BendRateZ("Bend Rate Z", Float) = 0
    }

    SubShader {

		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		// Utility functions
		#include "UnityCG.cginc"
		#include "TransformationShader.cginc"

        #pragma surface surf Lambert vertex:vert
		// === UNIFORM VARIABLES ===
		// Just copied from Shaderlab wrapper!

		// COMMON
		float4 _MaxExtents;

		// TWIST
		float _TwistAngleX;
		float _TwistAngleY;
		float _TwistAngleZ;

		// STRETCH
		float _StretchStrengthX;
		float _StretchStrengthY;
		float _StretchStrengthZ;
		float _StretchAmountX;
		float _StretchAmountY;
		float _StretchAmountZ;

		// BEND
		float _XMin;
		float _XMax;
		float _X0;
		float _BendRateX;

		float _YMin;
		float _YMax;
		float _Y0;
		float _BendRateY;

		float _ZMin;
		float _ZMax;
		float _Z0;
		float _BendRateZ;

		// FFD
		bool _IsOriginDown;

		// bezier curves degrees
		int _L;
		int _M;
		int _N;

		// Control points in FFD grid
		// NB: At most FFD_MAX_PTS points can be used!
		// This is done since dynamically sized arrays are not supported in hlsl
		float4 _ControlPoints[FFD_MAX_PTS];

		// Because of the surface shaders limitations, a non-empty input struct
		// must always be defined, even if not used.
		struct Input {
		  int ignore_this;
		};

        // NB: the input vertex data MUST be of type appdata_full, even if all
        // additional data are not used
		void vert (inout appdata_full v) {
			// Apply FREE-FORM DEFORMATION (lattice)
			// NB: Due to the nature of transformation, we pass in extents greater than the real ones.
			// Otherwise, we can encounter a situation of the type: 0^0 which result in NaNs propagating
			DoFFD(v, _IsOriginDown, _L, _M, _N, _ControlPoints, float4(_MaxExtents.xyz * 1.2f, _MaxExtents.w));

			// The order is as follows
			// TWIST - STRETCH - BEND, for each along X,Y,Z respectively
			DoTwist(v, X_AXIS, _TwistAngleX, _MaxExtents);
			DoTwist(v, Y_AXIS, _TwistAngleY, _MaxExtents);
			DoTwist(v, Z_AXIS, _TwistAngleZ, _MaxExtents);

			DoStretch(v, X_AXIS, _StretchAmountX, _StretchStrengthX, _MaxExtents );
			DoStretch(v, Y_AXIS, _StretchAmountY, _StretchStrengthY, _MaxExtents );
			DoStretch(v, Z_AXIS, _StretchAmountZ, _StretchStrengthZ, _MaxExtents );

			DoBend(v, X_AXIS, _XMin, _XMax, _X0, _BendRateX, _MaxExtents );
			DoBend(v, Y_AXIS, _YMin, _YMax, _Y0, _BendRateY, _MaxExtents );
			DoBend(v, Z_AXIS, _ZMin, _ZMax, _Z0, _BendRateZ, _MaxExtents );
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = half3(0,0.5,0.5);
		}

      ENDCG
    }

    Fallback "Diffuse"
  }
