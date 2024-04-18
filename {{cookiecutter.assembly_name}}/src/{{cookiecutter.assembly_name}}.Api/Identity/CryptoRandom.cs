#region License
//
// [1] http://msdn.microsoft.com/en-us/magazine/cc163367.aspx
//
// Original work by Stephen Toub and Shawn Farkas
// Published in "Tales from the CryptoRandom"; September 2007 issue of MSDN Magazine
//
// 
#endregion

using System.Security.Cryptography;

namespace {{cookiecutter.assembly_name}}.Api.Identity;

public class CryptoRandom : Random
{
     {%- if cookiecutter.include_azure == "yes" -%}
    private readonly RandomNumberGenerator _randomizer = RandomNumberGenerator.Create();
    private readonly byte[] _buffer = new byte[sizeof( int )];

    public override int Next()
    {
        _randomizer.GetBytes( _buffer );
        return BitConverter.ToInt32( _buffer, 0 ) & 0x7FFFFFFF;
    }

    public override int Next( int maxValue )
    {
        if ( maxValue < 0 )
            throw new ArgumentOutOfRangeException( nameof( maxValue ) );

        return Next( 0, maxValue );
    }

    public override int Next( int minValue, int maxValue )
    {
        if ( minValue > maxValue )
            throw new ArgumentOutOfRangeException( nameof( minValue ) );

        if ( minValue == maxValue )
            return minValue;

        const long randomRange = 1 + (long) uint.MaxValue;
        long targetRange = maxValue - minValue;

        while ( true )
        {
            _randomizer.GetBytes( _buffer );
            var rand = BitConverter.ToUInt32( _buffer, 0 );

            // Detect biased results and retry
            //
            // Given perfectly random input bits in buffer, mapping to a min and max range works
            // perfectly when the target range is an even power of two. This is because the UInt32 
            // values generated from GetBytes have 2 ^ 32 possible values; to evenly distribute these 
            // among all of the values in the target range, the size of that target range must evenly 
            // divide the input range, and with a power of two such as 2 ^ 32, that will only happen 
            // if the size of the target range is also a power of two. When the size of the target 
            // range is not a power of two, the "random" results will start to weigh more heavily 
            // toward the lower values.

            if ( rand >= randomRange - randomRange % targetRange )
                continue; // ignore biased values

            return (int) (minValue + rand % targetRange);
        }
    }

    public override void NextBytes( byte[] buffer )
    {
        if ( buffer == null )
            throw new ArgumentNullException( nameof( buffer ) );

        _randomizer.GetBytes( buffer );
    }

    public override double NextDouble()
    {
        _randomizer.GetBytes( _buffer );
        var rand = BitConverter.ToUInt32( _buffer, 0 );

        return rand / (1.0 + uint.MaxValue);
    }
    {% endif %}
}
