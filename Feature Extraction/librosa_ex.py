import librosa
import numpy as np

filename = './200-BPM.wav'
y, sr = librosa.load(filename)
print("El sample rate es: ", sr)

stft = librosa.stft(y)
frequencies = librosa.fft_frequencies(sr=sr)

print("Las frecuencias presentes en el archivo de audio son:", frequencies)
6
bpm = librosa.beat.tempo(y=y, sr=sr)
print("El BPM estimado es:", bpm[0], "beats por minuto.")

# tono = librosa.hz_to_midi(librosa.pitch_tuning(y, sr))
# nota = librosa.midi_to_note(tono)

# print("El tono principal del archivo de audio es:", nota)