import wave
import struct
import math
import random
import os

sounds_dir = r"j:\GMDD\Asignment Games\zomboid-survival\Assets\Sounds"
os.makedirs(sounds_dir, exist_ok=True)

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

def s_gunshot():
    # White noise with very fast exponential decay, plus a low frequency thump
    samples = []
    sample_rate = 44100
    duration = 0.5
    for i in range(int(sample_rate * duration)):
        t = i / sample_rate
        env = math.exp(-15 * t)
        noise = random.uniform(-1.0, 1.0)
        thump = math.sin(2 * math.pi * 150 * t * math.exp(-10 * t))
        samples.append((noise * 0.7 + thump * 0.5) * env)
    generate_wav("gunshot.wav", samples)

def s_footstep(num):
    # Short low frequency thud + some high freq grit
    samples = []
    sample_rate = 44100
    duration = 0.2
    for i in range(int(sample_rate * duration)):
        t = i / sample_rate
        env = math.exp(-30 * t)
        thud = math.sin(2 * math.pi * (80 + random.uniform(-10, 10)) * t)
        grit = random.uniform(-1.0, 1.0) * 0.3
        samples.append((thud * 0.6 + grit) * env)
    generate_wav(f"footstep_{num}.wav", samples)

def s_zombie_roar():
    # Slow FM synthesis for a throat grumble / roar
    samples = []
    sample_rate = 44100
    duration = 1.5
    for i in range(int(sample_rate * duration)):
        t = i / sample_rate
        # Envelope: slow attack, slow release
        env = math.sin(math.pi * (t / duration)) ** 2
        # Carrier sine wave modulated by low frequency random noise
        modulation = random.uniform(-0.5, 0.5) + math.sin(2 * math.pi * 5 * t)
        roar = math.sin(2 * math.pi * 60 * t + modulation * 10)
        samples.append(roar * env * 0.8)
    generate_wav("zombie_roar.wav", samples)

def s_zombie_attack():
    # Quick swipe / bite sound (noise sweep + thud)
    samples = []
    sample_rate = 44100
    duration = 0.4
    for i in range(int(sample_rate * duration)):
        t = i / sample_rate
        env = math.exp(-10 * t)
        noise = random.uniform(-1.0, 1.0)
        swipe = noise * math.sin(math.pi * t / duration)
        thud = math.sin(2 * math.pi * 40 * t)
        samples.append((swipe * 0.5 + thud * 0.5) * env)
    generate_wav("zombie_attack.wav", samples)

s_gunshot()
s_footstep(1)
s_footstep(2)
s_zombie_roar()
s_zombie_attack()
