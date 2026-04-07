import wave
import struct
import math
import random
import os

sounds_dir = r"j:\GMDD\Asignment Games\zomboid-survival\Assets\Resources"
os.makedirs(sounds_dir, exist_ok=True)

def generate_bgm():
    # Eerie low frequency drone + wind
    samples = []
    sample_rate = 44100
    duration = 8.0 # 8 second loop
    for i in range(int(sample_rate * duration)):
        t = i / sample_rate
        # Low drone
        drone = math.sin(2 * math.pi * 50 * t) * 0.4
        drone += math.sin(2 * math.pi * 55 * t) * 0.2
        
        # Wind / noise with slow modulation
        wind_mod = (math.sin(2 * math.pi * 0.2 * t) + 1) * 0.5
        wind = random.uniform(-1.0, 1.0) * 0.1 * wind_mod
        
        s = drone + wind
        samples.append(s)
        
    filepath = os.path.join(sounds_dir, "bgm.wav")
    with wave.open(filepath, 'w') as wav_file:
        wav_file.setnchannels(1)
        wav_file.setsampwidth(2)
        wav_file.setframerate(sample_rate)
        for sample in samples:
            # Clamp sample to -1 to 1
            s = max(-1.0, min(1.0, sample))
            wav_file.writeframes(struct.pack('h', int(s * 32767.0)))
    print("Generated bgm.wav")

generate_bgm()
