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



# Load a file and resample to 11 KHz ------------------------------DA ERROR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
# filename = librosa.ex('200-BPM.wav')
# y, sr = librosa.load(filename, sr=11025)

def tempogram(samples, sr):
    # Calcular el tempograma
    tempogram = librosa.feature.tempogram(y=samples, sr=sr, hop_length=512)
    print(tempogram)
    # Graficar el tempograma
    plt.imshow(tempogram, origin='lower', aspect='auto', cmap='inferno')
    plt.title("Tempograma de la canción")
    plt.xlabel("Tiempo (frames)")
    plt.ylabel("Frecuencia (bpm)")
    plt.show() 

'''
Función para obtener la onda y la frecuencia de muestreo de un archivo de audio
'''
def loadWave(filename):
    wave, sample_rate = librosa.load(filename)
    return wave, sample_rate

#--------------------FUNCIONES PARA EXTRACCIÓN DE CARACTERÍSTICAS-------------------------------
'''
Devuelve los Pulos Por Minuto (BPM) de una onda
Devuelve los instantes en segundos donde se producen los beats 
'''
def get_bits_per_minute(samples, sr):  #ESTA ABAJO DUPLICAO +-l.64
    # Calcular el tempo (BPM) y los frames de los beats
    bpm, beats = librosa.beat.beat_track(y=samples, sr=sr)
    # Convertir los frames de los beats a tiempos en segundos
    beat_times = librosa.frames_to_time(beats, sr=sr)
    return bpm, beat_times

'''
Devuelve un array de bool indicando cuándo pasa la onda por 0
Devuelve el número de veces que la señal cruza el eje horizontal por cero.
'''
def zero_crossing(wave, ini = 0, fin = 0):
    if (fin == 0): fin = wave.shape[0]
    zero_crossings = librosa.zero_crossings(wave[ini:fin], pad = False)
    total_crossings = sum(zero_crossings)
    return zero_crossings, total_crossings

#------------------------------------LA REVISIÓN DE MÉTODOS VA POR AQUÍ-------------------------------------------
'''
Calculo del zero crossing en toda una muestra
El zero crossing rate indica el numero de veces que la señal cruza el eje horizontal por cero.
'''
def zero_crossing_interval(x):
    zcrs = librosa.feature.zero_crossing_rate(x)
    return zcrs

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
def rmse(x):
    hop_length = 256 # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    rmse = librosa.feature.rms(x, frame_length=frame_length, hop_length=hop_length, center=True)
    return rmse


'''
Gráfica de la Energía y Rmse junto a la onda
'''
def graphic_energy_and_rmse(x, sr):
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
STFT - Transformada corta de Fourier (Short-Time Fourier Transform)
'''
def stft(x, sr):
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
def spectogram(X, sr):
    S = librosa.amplitude_to_db(abs(X))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(S, sr=sr, x_axis='time', y_axis='linear')

'''
La escala Mel relaciona la frecuencia percibida, o tono, de un tono puro con su frecuencia medida real.
'''
def melSpectogram(x, sr):
    S = librosa.feature.melspectrogram(x, sr=sr, n_fft=4096, hop_length=256)
    logS = librosa.amplitude_to_db(S)
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(logS, sr=sr, x_axis='time', y_axis='mel')

    # OTRA FORMA
    # # Compute the mel-scaled spectrogram
    # melody = librosa.feature.melspectrogram(x, sr=sr)
    # # Convert the spectrogram to decibels
    # db_melody = librosa.amplitude_to_db(melody, ref=np.max)
    # # Plot the spectrogram
    # librosa.display.specshow(db_melody, sr=sr, x_axis='time', y_axis='mel')
    # plt.colorbar(format='%+2.0f dB')
    # plt.title('Mel-frequency spectrogram')
    # plt.tight_layout()
    # plt.show()

def constantQTransform(x, sr):
    fmin = librosa.midi_to_hz(36)
    C = librosa.cqt(x, sr=sr, fmin=fmin, n_bins=72)
    logC = librosa.amplitude_to_db(abs(C))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(logC, sr=sr, x_axis='time', y_axis='cqt_note', fmin=fmin, cmap='coolwarm')
    plt.show()

def melFrequencyCepstralCoefficients(x, sr):
    mfccs = librosa.feature.mfcc(x, sr=sr)
    print(mfccs.shape)
    librosa.display.specshow(mfccs, sr=sr, x_axis='time')
    plt.show()

def normalize(x, axis=0):
    return sklearn.preprocessing.minmax_scale(x, axis=axis)

def spectralCentroid(x, sr):
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

def spectralContrast(x, sr):
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

#-----------------------------PRUEBAS----------------------------------

def main():
    # # Cargar una señal
    # x, sr = librosa.load('200-BPM.wav') # frecuencia de muestreo
    # x.shape # Tamaño
    # librosa.get_duration(x, sr) # duracion

    # Carga la canción en un array de muestras
    filename = "200-BPM.wav"
    samples, sr = librosa.load(filename)
    tempogram(samples, sr)


filename = "200-BPM.wav"
wave, sample_rate = loadWave(filename)

def pruebaBeats():
    bpm, beats = get_bits_per_minute(wave, sample_rate)
    print(bpm)
    librosa.display.waveshow(wave)
    plt.ylim(-1,1)
    plt.tight_layout()
    array_y = np.zeros(beats.shape)
    plt.plot(beats, array_y, 'r+')
    plt.grid()
    plt.show()

def pruebaPasoPorCeros():
    zero_crossing, total_crossings = zero_crossing(wave)
    print("Total corssings: ", total_crossings)


print(zero_crossing(wave)[1])
print(sum(zero_crossing_interval(wave)))