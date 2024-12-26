// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("dsRHZHZLQE9swA7AsUtHR0dDRkWcKhXCQ17Y+4DWJoPW8U/V7UdXUqZydK1H193H3sOfrzjrL/dU2gJ60EE9jfvq+UcRfqPwm/rXxihcH1PER0lGdsRHTETER0dG+tjrj/ynxuVo6rapHDMPyrmSvoUU+moqhkPscFtet304CpTaTvXK7k+vOcmrF6ljxIcA5uj5cFBDHfYg2Wi6Fp/6Bxw4BwMIls7uUPeMPX8m7EdmHMOT1sRNauAe0Ynq9j8xFp5UB452z4J5PtMndp65X20rpXmNJfuLwts3q2ZmgZxGh2D4NIlkzlyVm7FHBVVDGNf3rYi0YCJ2WtQUGWVsee/jmiGGYbV5imFt52dnMGA0RMLFIsnhEHUKlPn32ml1UURFR0ZH");
        private static int[] order = new int[] { 0,11,5,3,11,5,6,10,13,9,12,13,13,13,14 };
        private static int key = 70;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
