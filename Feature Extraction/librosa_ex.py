import librosa
import numpy as np

filename = './g.mp3'
y, sr = librosa.load(filename)
print("El sample rate es: ", sr)

stft = librosa.stft(y)
frequencies = librosa.fft_frequencies(sr=sr)

print("Las frecuencias presentes en el archivo de audio son:", frequencies)

bpm = librosa.beat.tempo(y=y, sr=sr)
print("El BPM estimado es:", bpm[0], "beats por minuto.")
