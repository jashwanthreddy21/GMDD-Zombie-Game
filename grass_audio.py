import wave
import struct
import math
import random
import os

sounds_dir = r"j:\GMDD\Asignment Games\zomboid-survival\Assets\Resources"

def generate_wav(filename, samples, sample_rate=44100):
    filepath = os.path.join(sounds_dir, filename)
    with wave.open(filepath, 'w') as wav_file:
        wav_file.setnchannels(1)
        wav_file.setsampwidth(2)
        wav_file.setframerate(sample_rate)
        for sample in samples:
            s = max(-1.0, min(1.0, sample))
            wav_file.writeframes(struct.pack('h', int(s * 32767.0)))
    print(f"Generated {filename}")

def grass_footstep(num):
    samples = []
    sr = 44100
    duration = 0.3
    for i in range(int(sr * duration)):
        t = i / sr
        
        # Envelope: Attack to 0.05s, then decay
        if t < 0.05:
            env = t / 0.05
        else:
            env = math.exp(-12 * (t - 0.05))
            
        # Base noise for rustling grass
        noise = random.uniform(-1.0, 1.0)
        
        # High pass filter effect (crinkle):
        # We modulate the noise with a high-pitched sine wave
        # This gives it a "crunchy" leaf/grass texture
        crinkle = math.sin(2 * math.pi * 1200 * t) * noise
        
        # Another lower crunch for thickness
        crunch = math.sin(2 * math.pi * 300 * t) * noise
        
        # Add a very subtle low-end soft thud
        thud = math.sin(2 * math.pi * 50 * t) * 0.05
        
        # Differentiate num 1 and 2
        if num == 1:
            crinkle *= 1.2
            crunch *= 0.8
        else:
            crinkle *= 0.8
            crunch *= 1.2

        val = (crinkle * 0.3 + crunch * 0.2 + thud) * env
        samples.append(val)
        
    generate_wav(f"footstep_{num}.wav", samples)

grass_footstep(1)
grass_footstep(2)
