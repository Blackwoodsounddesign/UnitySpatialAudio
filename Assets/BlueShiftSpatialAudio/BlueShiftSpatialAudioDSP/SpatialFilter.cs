using System;
using AudioFXToolkitDSP;

public class SpatialFilter : BiquadFilter
{

    public void SetFilterParameters(int sample_rate, float frequency, float dBgain = 0f, float Q = 0.707f)
    {


        if (dBgain < 0)
        {
            //intermediate
            float A = (float)Math.Pow(10, (dBgain / 40));
            float w0 = (float)(2 * Math.PI * frequency / sample_rate);
            float alpha = (float)(Math.Sin(w0) * 0.5 * Math.Sqrt((A + 1 / A) * (1 / Q - 1) + 2));

            //coefficents from RBJ cookbook
            float b0 = (float)(A * (A + 1 + (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha));
            float b1 = (float)(-2 * A * ((A - 1) + (A + 1) * Math.Cos(w0)));
            float b2 = (float)(A * ((A + 1) + (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha));
            float a0 = (float)((A + 1) - (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha);
            float a1 = (float)(2 * ((A - 1) - ((A + 1) * Math.Cos(w0))));
            float a2 = (float)((A + 1) - (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha);

            SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);
        }
        else
        {

            float K = (float)Math.Tan((Math.PI * frequency) / sample_rate);
            float V0 = (float)Math.Pow(10, dBgain / 20);

            if (V0 < 1)
                V0 = 1 / V0;

                float b0 = (1 + ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float b1 = (2 * (K * K - 1)) / (1 + ((1 / Q) * K) + K * K);
                float b2 = (1 - ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float a0 = 1;
                float a1 = b1;
                float a2 = (1 - ((1 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);

                SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);
        }
    }
}
