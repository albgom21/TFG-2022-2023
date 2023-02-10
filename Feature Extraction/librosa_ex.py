import librosa
import librosa.display
import sklearn
import numpy as np
import matplotlib.pyplot as plt

# filename = '200-BPM.wav'
# y, sr = librosa.load(filename)
# print("El sample rate es: ", sr)

# stft = librosa.stft(y)
# frequencies = librosa.fft_frequencies(sr=sr)

# print("Las frecuencias presentes en el archivo de audio son:", frequencies)
# bpm = librosa.beat.tempo(y=y, sr=sr)
# print("El BPM estimado es:", bpm[0], "beats por minuto.")

# tono = librosa.hz_to_midi(librosa.pitch_tuning(y, sr))
# nota = librosa.midi_to_note(tono)
# print("El tono principal del archivo de audio es:", nota)

# Cargar una señal
x, sr = librosa.load('200-BPM.wav') # frecuencia de muestreo
x.shape # Tamaño
librosa.get_duration(x, sr) # duracion

# Load a file and resample to 11 KHz ------------------------------DA ERROR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
# filename = librosa.ex('200-BPM.wav')
# y, sr = librosa.load(filename, sr=11025)


#--------------------FUNCIONES PARA EXTRACCIÓN DE CARACTERÍSTICAS-------------------------------
'''
Función simple que construye un vector de característica bidimensional a partir de una señal,
calculando el numero de cruces por cero de la señal y el centroide del espectro

Si queremos agregar todos los vectores de características entre 
las señales de una colección, podemos usar una list comprehension de la siguiente manera:
kick_features = np.array([extract_features(x) for x in kick_signals])
'''
def extract_features(signal):
    return [
        librosa.feature.zero_crossing_rate(signal)[0, 0], # Numero de cruces por cero
        librosa.feature.spectral_centroid(signal)[0, 0],  # Centroide del espectro
    ]


'''
Escalamiento de las caracteristicas, normalizar características a un rango común (-1, 1)
'''
def feature_scaling(feature_table):
    scaler = sklearn.preprocessing.MinMaxScaler(feature_range=(-1, 1))
    training_features = scaler.fit_transform(feature_table)
    return training_features

'''
La Energia de una señal corresponde a suma total de las magnitudes de la señal
'''
def energy(x):
    #Calcular el la energia por segmentos pequeños (short-time) usando list comprehension:
    hop_length = 256 # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    energy = np.array([
    sum(abs(x[i:i+frame_length]**2)) for i in range(0, len(x), hop_length)])
    return energy

'''
root-mean-square energy (RMSE)
'''
def rmse():
    hop_length = 256 # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    rmse = librosa.feature.rms(x, frame_length=frame_length, hop_length=hop_length, center=True)
    return rmse


'''
Gráfica de la Energía y Rmse junto a la onda
'''
def graphic_energy_and_rmse():
    hop_length = 256   # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    frames = range(len(energy))
    t = librosa.frames_to_time(frames, sr=sr, hop_length=hop_length)
    plt.figure(figsize=(14, 5))
    librosa.display.waveplot(x, sr=sr, alpha=0.4)       # mostrar onda
    plt.plot(t, energy/energy.max(), 'r--')             # normalizada para la visualizacion
    plt.plot(t[:len(rmse)], rmse/rmse.max(), color='g') # normalizada para la visualizacion
    plt.legend(('Energia', 'RMSE'))


'''
Calculo del zero crossing en un intervalo
El zero crossing rate indica el numero de veces que la señal cruza el eje horizontal por cero.
'''
def zero_crossing(x, ini, fin):
    zero_crossings = librosa.zero_crossings(x[ini:fin], pad=False)
    return sum(zero_crossings)


'''
Calculo del zero crossing en toda una muestra
El zero crossing rate indica el numero de veces que la señal cruza el eje horizontal por cero.
'''
def zero_crossing_interval(x):
    zcrs = librosa.feature.zero_crossing_rate(x)
    return zcrs

'''
STFT - Transformada corta de Fourier (Short-Time Fourier Transform)
'''
def stft():
    hop_length = 512 #incremento
    n_fft = 2048 #Tamaño del segmento

    # Para convertir el tamaño del segmento y el incremento en segundos
    float(hop_length)/sr # [=] segundos
    float(n_fft)/sr # [=] segundos

    X = librosa.stft(x, n_fft=n_fft, hop_length=hop_length)
    return X

'''
El Espectrograma muestra la intensidad de las frecuencias a lo largo del tiempo.
Un espectrograma es simplemente la magnitud al cuadrado de la STFT (Short-time Fourier Transform)
La percepción humana de la intensidad del sonido es de naturaleza logarítmica.
Por lo tanto, a menudo nos interesa la amplitud en esacala logaritmica (db)
'''
def spectogram(X):
    S = librosa.amplitude_to_db(abs(X))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(S, sr=sr, x_axis='time', y_axis='linear')

'''
La escala Mel relaciona la frecuencia percibida, o tono, de un tono puro con su frecuencia medida real.
'''
def melSpectogram(x):
    S = librosa.feature.melspectrogram(x, sr=sr, n_fft=4096, hop_length=256)
    logS = librosa.amplitude_to_db(S)
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(logS, sr=sr, x_axis='time', y_axis='mel');

def constantQTransform():
    fmin = librosa.midi_to_hz(36)
    C = librosa.cqt(x, sr=sr, fmin=fmin, n_bins=72)
    logC = librosa.amplitude_to_db(abs(C))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(logC, sr=sr, x_axis='time', y_axis='cqt_note', fmin=fmin, cmap='coolwarm')
    plt.show()

def melFrequencyCepstralCoefficients():
    mfccs = librosa.feature.mfcc(x, sr=sr)
    print(mfccs.shape)
    librosa.display.specshow(mfccs, sr=sr, x_axis='time')
    plt.show()

def normalize(x, axis=0):
    return sklearn.preprocessing.minmax_scale(x, axis=axis)

def spectralCentroid():
    spectral_centroids = librosa.feature.spectral_centroid(x, sr=sr)[0]
    print(spectral_centroids.shape)
    frames = range(len(spectral_centroids))
    t = librosa.frames_to_time(frames)
    librosa.display.waveshow(x, sr=sr, alpha=0.4)
    plt.plot(t, normalize(spectral_centroids), color='r'); # normalizacion para proposito de visualizacion
    plt.show()


def spectralBandwidth():
    # spectral_bandwidth_2 = librosa.feature.spectral_bandwidth(x+0.01, sr=sr)[0]
    # spectral_bandwidth_3 = librosa.feature.spectral_bandwidth(x+0.01, sr=sr, p=3)[0]
    # spectral_bandwidth_4 = librosa.feature.spectral_bandwidth(x+0.01, sr=sr, p=4)[0]
    # librosa.display.waveshow(x, sr=sr, alpha=0.4)
    # plt.plot(x, normalize(spectral_bandwidth_2), color='r')
    # plt.plot(x, normalize(spectral_bandwidth_3), color='g')
    # plt.plot(x, normalize(spectral_bandwidth_4), color='y')
    # plt.legend(('p = 2', 'p = 3', 'p = 4'))
    # plt.show()
    return

def spectralContrast():
    spectral_contrast = librosa.feature.spectral_contrast(x, sr=sr)
    print(spectral_contrast.shape)
    plt.imshow(normalize(spectral_contrast, axis=1), aspect='auto', origin='lower', cmap='coolwarm');
    plt.show()


def spectralRolloff():
    # spectral_rolloff = librosa.feature.spectral_rolloff(x+0.01, sr=sr)[0]
    # librosa.display.waveplot(x, sr=sr, alpha=0.4)
    # plt.plot(t, normalize(spectral_rolloff), color='r')
    # plt.show()
    return



