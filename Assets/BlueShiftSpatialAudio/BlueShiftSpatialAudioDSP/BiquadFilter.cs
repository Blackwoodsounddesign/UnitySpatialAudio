namespace AudioFXToolkitDSP
{

    /****************
     * BiquadFilter Class 
     * --------------
     * A generic DSP biquad function with a setter.
     * This is used throughout the various filters. A filter design tool can found in Max/MSP and online. To use this filter, supply it with coefficients of the filter you are trying to create.
     * The other filters in this mainly use this the underlying DSP and simply algorithmically generate the coefficients. 
     * 
     * \note Frustratingly, many of the online coefficient generators use slightly different variable names.
     * These ones are taken from Max/MSP. In this link, the coefficient names are reversed: https://arachnoid.com/BiQuadDesigner/index.html 
     * 
     * <br>
     * https://en.wikipedia.org/wiki/Digital_biquad_filter
     */

    public class BiquadFilter
    {
        private float a0, a1, a2, b1, b2;
        private float x1, x2, y1, y2;

        /// <summary>
        /// This is a biquad filter algorithm. Set the coefficients using any number of filter coefficient functions.
        /// Included in AudioFXToolkitDSP is a number of these functions.
        ///
        ///     Included:
        ///     
        ///         PeakNotchFilter
        ///         BandPassFilter
        ///         LowPassFilter
        ///         HighPassFilter
        ///         LowShelfFilter
        ///         HighShelfFilter
        ///
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The floating point input sample.
        /// 
        /// <returns> The float value after passing through the filter. </returns>

        public float Filter(float inputSample)
        {
            // That sweet Biquad code.  
            float result = a0 * inputSample + a1 * x1 + a2 * x2 - b1 * y1 - b2 * y2;

            // shift x1 to x2, sample to x1
            x2 = x1;
            x1 = inputSample;

            // shift y1 to y2, result to y1
            y2 = y1;
            y1 = result;

            return result;
        }

        // Coefficent setter.  
        public void SetCoefficents(float _a0, float _a1, float _a2, float _b1, float _b2)
        {
            a0 = _a0; a1 = _a1; a2 = _a2; b1 = _b1; b2 = _b2;
        }
    }
}