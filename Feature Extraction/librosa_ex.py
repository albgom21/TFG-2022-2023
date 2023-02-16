import librosa
import librosa.display
import sklearn
import sklearn.preprocessing
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

'''
Muestra un tempograma

Un tempograma es una representación gráfica de la estructura temporal de una canción,
donde los beats se representan como picos en el gráfico. Los picos más altos representan
beats más fuertes y los picos más bajos representan beats más débiles.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)

'''
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
Carga un archivo de audio para obtener los samples y su frecuencia de muestreo

Parametros
----------
filename : cadena que representa la ruta del audio

Returns
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def load_Wave(filename):
    samples, sr = librosa.load(filename)
    return samples, sr

#--------------------FUNCIONES PARA EXTRACCIÓN DE CARACTERÍSTICAS-------------------------------
'''
Obtener beats a lo largo del tiempo

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)

Returns
----------
bpm : Pulsos Por Minuto (BPM) de una onda
beat_times: los instantes en segundos donde se producen los beats 
'''
def get_beats_in_timeline(samples, sr):  
    # Calcular el tempo (BPM) y los frames de los beats
    bpm, beats = librosa.beat.beat_track(y=samples, sr=sr)
    # Convertir los frames de los beats a tiempos en segundos
    beat_times = librosa.frames_to_time(beats, sr=sr)
    return bpm, beat_times

'''
Obtener beats a lo largo del tiempo

Parametros
----------
samples : muestras del audio
ini : inicio de la muestra para analizar
fin : fin de la muestra para analizar
Returns
----------
zero_crossings : array de bool indicando cuándo pasa la onda por 0
total_crossings : número de veces que la señal cruza el eje horizontal por cero en el rango dado
'''
def zero_crossing(samples, ini = 0, fin = 0):
    if (fin == 0): fin = samples.shape(0) 
    zero_crossings = librosa.zero_crossings(samples[ini:fin], pad = False)
    total_crossings = sum(zero_crossings)
    return zero_crossings, total_crossings

#------------------------------------LA REVISIÓN DE MÉTODOS VA POR AQUÍ-------------------------------------------
'''
Calculo del zero crossing en toda una muestra

El zero crossing rate indica el cuando la señal cruza el eje horizontal por cero en frames

Parametros
----------
samples : muestras del audio

Returns
----------
zcrs : array de bool indicando cuándo pasa la onda por 0

'''
def zero_crossing_interval(samples):
    zcrs = librosa.feature.zero_crossing_rate(samples)
    return zcrs


'''
--------SE PUEDE QUITAR PORQUE SE OBTIENEN POR SEPARADO CON OTRAS FUNCIONES---------

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
--------COMPROBAR QUE FUNCIONE CON UN ARRAY YA QUE LA TABLA NO SE USARÁ--------------
Escalamiento de las caracteristicas, normalizar características a un rango común (-1, 1)

Parametros
----------
feature_table : tabla con las caracteríticas del audio

Returns
----------
training_features : tabla de características normalizada
'''
def feature_scaling(feature_table):
    scaler = sklearn.preprocessing.MinMaxScaler(feature_range=(-1, 1))
    training_features = scaler.fit_transform(feature_table)
    return training_features

'''
Calcular la energia por segmentos pequeños

La energia de una señal corresponde a suma total de las magnitudes de la señal

Parametros
----------
samples : muestras del audio

Returns
----------
energy : array de frames con su energía
'''
def energy(samples):
    hop_length = 256    # tamaño del incremento
    frame_length = 512  # tamaño del segmento
    energy = np.array([sum(abs(samples[i:i+frame_length]**2)) for i in range(0, len(samples), hop_length)])
    return energy

'''
root-mean-square energy (RMSE)

Parametros
----------
samples : muestras del audio

Returns
----------
rmse : array de frames con su RMSE
'''
def rmse(samples):
    hop_length = 256   # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    rmse = librosa.feature.rms(samples, frame_length=frame_length, hop_length=hop_length, center=True)
    return rmse


'''
Gráfica de la Energía y Rmse junto a la onda

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
energy : array de frames con su energía
rmse : array de frames con su RMSE
'''
def graphic_energy_and_rmse(samples, sr, energy, rmse):
    hop_length = 256   # tamaño del incremento
    frame_length = 512 # tamaño del segmento
    frames = range(len(energy))
    t = librosa.frames_to_time(frames, sr=sr, hop_length=hop_length)
    plt.figure(figsize=(14, 5))
    librosa.display.waveshow(samples, sr=sr, alpha=0.4) # mostrar onda
    plt.plot(t, energy/energy.max(), 'r--')             # normalizada para la visualizacion
    plt.plot(t[:len(rmse)], rmse/rmse.max(), color='g') # normalizada para la visualizacion
    plt.legend(('Energia', 'RMSE'))



'''
STFT - Transformada corta de Fourier (Short-Time Fourier Transform)

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)

Returns
----------
X : matriz en dos dimensiones que representa la energía de la señal en diferentes frecuencias a lo largo del tiempo (segmentos)
'''
def stft(samples, sr):
    hop_length = 512 #incremento
    n_fft = 2048     #Tamaño del segmento

    # Para convertir el tamaño del segmento y el incremento en segundos
    float(hop_length)/sr # [=] segundos
    float(n_fft)/sr # [=] segundos

    X = librosa.stft(samples, n_fft=n_fft, hop_length=hop_length)
    return X

'''
Muestra el espectrograma del audio

El espectrograma muestra la intensidad de las frecuencias a lo largo del tiempo.
Un espectrograma es simplemente la magnitud al cuadrado de la STFT (Short-time Fourier Transform)
La percepción humana de la intensidad del sonido es de naturaleza logarítmica.
Por lo tanto, a menudo nos interesa la amplitud en esacala logaritmica (db)

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def spectogram(samples, sr):
    S = librosa.amplitude_to_db(abs(samples))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(S, sr=sr, x_axis='time', y_axis='linear')

'''
Muestra el Mel-espectrograma del audio

La escala Mel relaciona la frecuencia percibida, o tono, de un tono puro con su frecuencia medida real.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def mel_Spectogram(samples, sr):
    S = librosa.feature.melspectrogram(samples, sr=sr, n_fft=4096, hop_length=256)
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
'''
Muestra Constant-Q transform

A diferencia de la transformada de Fourier, pero similar a la escala mel, 
la constant-Q transform utiliza un eje de frecuencia espaciado logaritmicamente.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def constant_Q_Transform(samples, sr):
    fmin = librosa.midi_to_hz(36)
    C = librosa.cqt(samples, sr=sr, fmin=fmin, n_bins=72)
    logC = librosa.amplitude_to_db(abs(C))
    plt.figure(figsize=(15, 5))
    librosa.display.specshow(logC, sr=sr, x_axis='time', y_axis='cqt_note', fmin=fmin, cmap='coolwarm')
    plt.show()

'''
Muestra MFCCs

Los Coeﬁcientes Cepstrales en las Frecuencias de Mel o MFCCs son coeﬁcientes
para la representación del habla basados en la percepción auditiva humana
A menudo se usa para describir el timbre.
Limpia la señal de ruido.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def mel_Frequency_Cepstral_Coefficients(samples, sr):
    mfccs = librosa.feature.mfcc(samples, sr=sr)
    print(mfccs.shape)
    librosa.display.specshow(mfccs, sr=sr, x_axis='time')
    plt.show()


'''
Normalizar datos para la visualizacion con la onda de audio
'''
def normalize(x, axis=0):
    return sklearn.preprocessing.minmax_scale(x, axis=axis)


'''
Muestra la onda del audio y el centroide espectral en una misma gráfica.

El centroide del espectro en el audio es una medida que indica la posición media de las frecuencias presentes en una señal de audio.
Es una medida de la tonalidad o el color de la señal.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)

Return
----------
matriz : 
'''
def spectral_centroid_v1(samples, sr, show = False):
    spectral_centroids = librosa.feature.spectral_centroid(samples+0.01, sr)[0] # samples+0.01 para evitar fallos en silencios
    # print(spectral_centroids.shape)
    frames = range(len(spectral_centroids))
    t = librosa.frames_to_time(frames)
    librosa.display.waveshow(samples, sr=sr, alpha=0.4)
    norm_spect_cent = normalize(spectral_centroids)

    if(show):
        plt.plot(t, norm_spect_cent, color='r'); # normalizacion para proposito de visualizacion
        plt.show()

    # Combinar los dos arrays en una matriz bidimensional
    matriz = np.column_stack((t, norm_spect_cent))
    return matriz


'''
Muestra la onda del audio y el centroide espectral en dos gráficas.

El centroide del espectro en el audio es una medida que indica la posición media de las frecuencias presentes en una señal de audio.
Es una medida de la tonalidad o el color de la señal.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def spectral_centroid_v2(samples, sr):
    # Calcular el centroide espectral
    spectral_centroids = librosa.feature.spectral_centroid(samples, sr)[0]

    # Calcular el tiempo en segundos
    times = librosa.frames_to_time(np.arange(len(spectral_centroids)), sr=sr)

    # Crear la figura
    plt.figure()

    # Agregar la onda del audio como primer subplot
    plt.subplot(2, 1, 1)
    librosa.display.waveshow(samples, sr=sr)

    # Agregar el centroide espectral como segundo subplot
    plt.subplot(2, 1, 2)
    plt.plot(times, spectral_centroids, c="red")

    # Mostrar la figura
    plt.tight_layout()
    plt.show()


'''
Muestra el Ancho de Banda Espectral 

El Ancho de Banda Espectral es una medida de la amplitud de las frecuencias que componen una señal de audio.
Específicamente, se refiere a la medida de la anchura de la distribución de energía espectral de la señal de audio.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)

'''
def spectral_Bandwidth(samples, sr):
    spectral_bandwidth_2 = librosa.feature.spectral_bandwidth(samples+0.01, sr=sr)[0]
    spectral_bandwidth_3 = librosa.feature.spectral_bandwidth(samples+0.01, sr=sr, p=3)[0]
    spectral_bandwidth_4 = librosa.feature.spectral_bandwidth(samples+0.01, sr=sr, p=4)[0]
    librosa.display.waveshow(samples, sr=sr, alpha=0.4)
    plt.plot(samples, normalize(spectral_bandwidth_2), color='r')
    plt.plot(samples, normalize(spectral_bandwidth_3), color='g')
    plt.plot(samples, normalize(spectral_bandwidth_4), color='y')
    plt.legend(('p = 2', 'p = 3', 'p = 4'))
    plt.show()
    
'''
Muestra el Contraste Espectral

El Spectral contrast considera los picos espectrales, y los 'valles' espectrales, y sus diferencias en cada sub-banda de frecuencia.

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def spectral_Contrast(samples, sr):
    spectral_contrast = librosa.feature.spectral_contrast(samples, sr=sr)
    print(spectral_contrast.shape)
    plt.imshow(normalize(spectral_contrast, axis=1), aspect='auto', origin='lower', cmap='coolwarm');
    plt.show()


'''
Muestra el Rolloff

El Spectral Rolloff es la frecuencia por debajo de la cual un porcentaje específico de la energía espectral total se encuentra, normalmente el 85%

Parametros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def spectral_Roll_off(samples, sr):
    spectral_rolloff = librosa.feature.spectral_rolloff(samples+0.01, sr=sr)[0]
    librosa.display.waveshow(samples, sr=sr, alpha=0.4)
    frames = range(len(spectral_rolloff))
    t = librosa.frames_to_time(frames)
    plt.plot(t, normalize(spectral_rolloff), color='r')
    plt.show()


#-----------------------------PRUEBAS----------------------------------

def pruebaBeats(samples, sr):
    print("INI")
    bpm, beats = get_beats_in_timeline(samples, sr)
    np.savetxt('beats.txt', beats, fmt='%.3f')
    print("FIN")
    # print(bpm)
    # librosa.display.waveshow(samples)
    # plt.ylim(-1,1)
    # plt.tight_layout()
    # array_y = np.zeros(beats.shape)
    # plt.plot(beats, array_y, 'r+')
    # plt.grid()
    # plt.show()


'''
Devuelve los valores del centroide espectral que coinciden con el tiempo
en los que se encuentran los beats 
'''
def sc_optimo(sc, beats):
   mascara_fila = np.isin(sc[:,0], beats)
   filtrada = sc[:,1][mascara_fila]
   return filtrada

def main():
    # # Cargar una señal
    # x, sr = librosa.load('200-BPM.wav') # frecuencia de muestreo
    # x.shape # Tamaño
    # librosa.get_duration(x, sr) # duracion

    # Carga la canción en un array de muestras
    filename = "200-BPM.wav"
    samples, sr = librosa.load(filename)
    # pruebaBeats(samples, sr)
    bpm, beats = get_beats_in_timeline(samples, sr)
    sc = spectral_centroid_v1(samples, sr)
    # np.savetxt('sc.txt', matriz, fmt='%.3f')
    resultado = sc_optimo(sc, beats)
    np.savetxt('scopt.txt', resultado, fmt='%.3f')

    # spectral_centroid(samples,sr)
    # print(energy(samples))

main()




    
# def pruebaPasoPorCeros():
#     zero_crossing, total_crossings = zero_crossing(wave)
#     print("Total corssings: ", total_crossings)

# print(zero_crossing(wave)[1])
# print(sum(zero_crossing_interval(wave)))