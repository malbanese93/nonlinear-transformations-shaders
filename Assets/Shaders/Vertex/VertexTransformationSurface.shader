Shader "Surface/Vertex" {
    Properties {
        // NB: VALUES SET BY UI        
		// === TWIST ===
		/*[Header(Twist)]
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
		_StretchStrengthZ("Stretch Strength Z", Range(0,3)) = 1*/

		/*[Space(10)]
		// === BEND ===
		[Header(Bend X)]
		// Params for bending region, expressed in percentage
		_BendXMin("Min %value X", Range(0,1)) = 0
		_BendXMax("Max %value X", Range(0,1)) = 1
		[Enum(Xmin,0,Halfway,1,Xmax,2)]_BendX0("Starting %value X (in affected region)", Float) = 0
		_BendAngleX("Angle at extremes X", Range(-360,360)) = 0
		[Space(5)]
		[Header(Bend Y)]
		_BendYMin("Min %value Y", Range(0,1)) = 0
		_BendYMax("Max %value Y", Range(0,1)) = 1
		[Enum(Ymin,0,Halfway,1,Ymax,2)]_BendY0("Starting %value Y (in affected region)", Float) = 0
		_BendAngleY("Angle at extremes Y", Range(-360,360)) = 0
		[Space(5)]
		[Header(Bend Z)]
		_BendZMin("Min %value Z", Range(0,1)) = 0
		_BendZMax("Max %value Z", Range(0,1)) = 1
		[Enum(Zmin,0,Halfway,1,Zmax,2)]_BendZ0("Starting %value Z (in affected region)", Float) = 0
		_BendAngleZ("Angle at extremes Z", Range(-360,360)) = 0*/
    }

    SubShader {

		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		// Utility functions
		#include "UnityCG.cginc"
		#include "TransformationShader.cginc"

        #pragma surface surf Lambert vertex:vert

		// COMMON
		float4 _MaxExtents;
        float4 _BoundsCenter;

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
		float _BendXMin;
		float _BendXMax;
		float _BendX0;
		float _BendAngleX;

		float _BendYMin;
		float _BendYMax;
		float _BendY0;
		float _BendAngleY;

		float _BendZMin;
		float _BendZMax;
		float _BendZ0;
		float _BendAngleZ;

		// FFD
		// bezier curves degrees
		int _L;
		int _M;
		int _N;

		// Control points in FFD grid
		// NB: At most FFD_MAX_PTS points can be used!
		// This is done since dynamically sized arrays are not supported in hlsl
		float4 _ControlPoints[FFD_MAX_PTS];

		struct Input {
            float3 vertexNormal;
		};

        // NB: the input vertex data MUST be of type appdata_full, even if all
        // additional data are not used
		void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);

            // First of all, translate all vertices by _BoundsCenter.
            // We do this since the mesh can be modelled around any pivot and this breaks
            // the portability of the transforms below. By using _BoundsCenter
            // we are sure to always start from the center of the mesh.
            v.vertex -= _BoundsCenter;

			// Apply FREE-FORM DEFORMATION (lattice)
            DoFFD(v, _L, _M, _N, _ControlPoints, _BoundsCenter, _MaxExtents);

			// The order is as follows
			// TWIST - STRETCH - BEND, for each along X,Y,Z respectively
			DoTwist(v, X_AXIS, _TwistAngleX, _MaxExtents);
			DoTwist(v, Y_AXIS, _TwistAngleY, _MaxExtents);
			DoTwist(v, Z_AXIS, _TwistAngleZ, _MaxExtents);

			DoStretch(v, X_AXIS, _StretchAmountX, _StretchStrengthX, _MaxExtents );
			DoStretch(v, Y_AXIS, _StretchAmountY, _StretchStrengthY, _MaxExtents );
			DoStretch(v, Z_AXIS, _StretchAmountZ, _StretchStrengthZ, _MaxExtents );

			DoBend(v, X_AXIS, _BendXMin, _BendXMax, _BendX0, _BendAngleX, _MaxExtents );
			DoBend(v, Y_AXIS, _BendYMin, _BendYMax, _BendY0, _BendAngleY, _MaxExtents );
			DoBend(v, Z_AXIS, _BendZMin, _BendZMax, _BendZ0, _BendAngleZ, _MaxExtents );

            // Restore coords wrt pivot
            v.vertex += _BoundsCenter;

            o.vertexNormal = abs(v.normal);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = half3(0.8f,0.8f,0.8f);
            o.Normal = UnityObjectToWorldNormal (IN.vertexNormal);
		}

      ENDCG
    }

    Fallback "Diffuse"
  }
