import wave
import struct
import math
import os

sounds_dir = r"j:\GMDD\Asignment Games\zomboid-survival\Assets\Resources"

def generate_wav(filename, samples, sample_rate=44100):
    filepath = os.path.join(sounds_dir, filename)
    with wave.open(filepath, 'w') as wav_file:
        wav_file.setnchannels(1)
        wav_file.setsampwidth(2)
        wav_file.setframerate(sample_rate)
        for sample in samples:
            # Clamp sample to -1 to 1
            s = max(-1.0, min(1.0, sample))
            wav_file.writeframes(struct.pack('h', int(s * 32767.0)))
    print(f"Generated {filename}")

def better_footstep(num):
    # A true "boot on concrete" knock: 
    # Quick attack, quick decay, multiple frequency components (300Hz, 100Hz, 50Hz)
    # Removing white noise entirely so it doesn't sound like a gunshot!
    samples = []
    sr = 44100
    duration = 0.2
    for i in range(int(sr * duration)):
        t = i / sr
        env = math.exp(-40 * t)
        
        # Base thud
        f1 = math.sin(2 * math.pi * 60 * t)
        # Mid knock
        f2 = math.sin(2 * math.pi * 300 * t) * 0.5
        # High tap
        f3 = math.sin(2 * math.pi * 800 * t) * 0.2
        
        if num == 1:
            f1 *= 1.1
        if num == 2:
            f2 *= 1.2
            
        samples.append((f1 + f2 + f3) * env * 0.6)
    generate_wav(f"footstep_{num}.wav", samples)

def spooky_bgm():
    # Minor chord progression a la classic zombie horror: A minor -> F major -> E major
    chords = [
        [110.0, 130.81, 164.81], # Am
        [87.31, 110.0, 130.81],  # F
        [82.41, 103.83, 123.47], # E
        [110.0, 130.81, 164.81]  # Am return
    ]
    
    samples = []
    sr = 44100
    duration_per_chord = 4.0
    for chord in chords:
        base_samples = [0] * int(sr * duration_per_chord)
        for freq in chord:
            for i in range(int(sr * duration_per_chord)):
                t = i / sr
                # Spooky slow attack and release
                env = math.sin(math.pi * (t / duration_per_chord))
                
                # Add some chorusing vibrato (frequency modulation)
                vibrato = math.sin(2 * math.pi * 4 * t) * 1.5
                wave_val = math.sin(2 * math.pi * (freq + vibrato) * t)
                
                # Add a spooky high octave bell-like harmonic hit on chord change
                bell_val = math.sin(2 * math.pi * (freq * 4) * t) * 0.1 * math.exp(-1.5 * t)
                
                base_samples[i] += (wave_val * 0.3 + bell_val) * env
        samples.extend(base_samples)
        
    generate_wav("bgm.wav", samples)

better_footstep(1)
better_footstep(2)
spooky_bgm()
