import os
import librosa
import librosa.display
import sklearn
import sklearn.preprocessing
import numpy as np
import matplotlib.pyplot as plt

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




#--------------------EXTRACCIÓN DE CARACTERÍSTICAS-------------------------------
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
Obtener la información del zero crossing

Parametros
----------
samples : muestras del audio

Returns
----------
zero_crossings : array de bool indicando cuándo pasa la onda por 0
total_crossings : número de veces que la señal cruza el eje horizontal por cero en el rango dado
'''
def zero_crossing(samples):
    zero_crossings = librosa.zero_crossings(samples, pad=False)
    total_crossings = sum(zero_crossings)
    return zero_crossings, total_crossings

'''
Calcula la tasa de cruces por cero en una señal de audio en cada frame

Parametros
----------
samples : muestras del audio

Returns
----------
zcrs : the fraction of zero crossings in each frame

'''
def zero_crossing_interval(samples):
    zcrs = librosa.feature.zero_crossing_rate(samples)
    return zcrs

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
    energy = np.array([sum(abs(samples[i:i+frame_length]**2))
                      for i in range(0, len(samples), hop_length)])
    return energy

'''
root-mean-square energy (RMSE)

Parametros
----------
samples : muestras del audio

Returns
----------
rmse : array de segundos con su RMSE
'''
def rmse(samples, frame_length = 512, hop_length=256):
  
    rmse = librosa.feature.rms(y=samples)
    # rmse = librosa.feature.rms(y=samples, frame_length=frame_length, hop_length=hop_length, center=True)
    # Convertir los valores RMS a dB
   
    rmse_db = librosa.amplitude_to_db(rmse, ref=np.max)
    tiempo_sec = librosa.times_like(rmse_db)

    matriz = np.column_stack((tiempo_sec, rmse_db[0]))
    
    # plt.plot(matriz[:,0], matriz[:,1], alpha=0.8, label='RMS')
    # plt.xlabel('Tiempo (s)')
    # plt.ylabel('Amplitud (dB)')
    # plt.title('RMSE a lo largo del tiempo')
    # plt.show()

    return matriz

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
    frame_length = 512  # tamaño del segmento
    frames = range(len(energy))
    t = librosa.frames_to_time(frames, sr=sr, hop_length=hop_length)
    plt.figure(figsize=(14, 5))
    librosa.display.waveshow(samples, sr=sr, alpha=0.4)  # mostrar onda
    # normalizada para la visualizacion
    plt.plot(t, energy/energy.max(), 'r--')
    # normalizada para la visualizacion
    plt.plot(t[:len(rmse)], rmse/rmse.max(), color='g')
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
    hop_length = 512  # incremento
    n_fft = 2048  # Tamaño del segmento

    # Para convertir el tamaño del segmento y el incremento en segundos
    float(hop_length)/sr  # [=] segundos
    float(n_fft)/sr  # [=] segundos

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
    S = librosa.feature.melspectrogram(
        samples, sr=sr, n_fft=4096, hop_length=256)
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
    librosa.display.specshow(logC, sr=sr, x_axis='time',
                             y_axis='cqt_note', fmin=fmin, cmap='coolwarm')
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

Parametros
----------
x : array de 1 dimensión
axis : dimensión del array a normalizar
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
def spectral_centroid_v1(samples, sr, show=False):
    spectral_centroids = librosa.feature.spectral_centroid(
        y=samples+0.01, sr=sr)[0]  # samples+0.01 para evitar fallos en silencios
    # print(spectral_centroids.shape)
    frames = range(len(spectral_centroids))
    t = librosa.frames_to_time(frames)
    librosa.display.waveshow(samples, sr=sr, alpha=0.4)
    norm_spect_cent = normalize(spectral_centroids)

    if(show):
        # normalizacion para proposito de visualizacion
        plt.plot(t, norm_spect_cent, color='r')
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
    spectral_bandwidth_2 = librosa.feature.spectral_bandwidth(
        samples+0.01, sr=sr)[0]
    spectral_bandwidth_3 = librosa.feature.spectral_bandwidth(
        samples+0.01, sr=sr, p=3)[0]
    spectral_bandwidth_4 = librosa.feature.spectral_bandwidth(
        samples+0.01, sr=sr, p=4)[0]
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
    plt.imshow(normalize(spectral_contrast, axis=1),
               aspect='auto', origin='lower', cmap='coolwarm')
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

'''
Devuelve los valores de la matriz que coinciden con el tiempo
en los que se encuentran los beats 
'''
def matriz_coord_beats(rmse, beats):
   mascara_fila = np.isin(rmse[:, 0], beats)
   filtrada = rmse[:, 1][mascara_fila]
   return filtrada

'''
Devuelve un array con los golpes de batería a lo largo del tiempo

Parámetros
----------
samples : muestras del audio
sr : frecuencia del audio (Sample Rate)
'''
def onset_detection(samples, sr):
    # Cargar la canción
    print(samples.size)
    y_perc = librosa.effects.percussive(samples, margin = 3.0)
    onset_frames = librosa.onset.onset_detect(y=y_perc, sr=sr)
    y_perc = librosa.frames_to_time(onset_frames, sr=sr)
    print(y_perc.size)
    sc = spectral_centroid_v1(y_perc, sr)
    
    return y_perc

#--------------------OTRAS FUNCIONES-------------------------------
def graves(y,sr):
    # Configurar los parámetros de la transformada de Fourier
    n_fft = 2048
    hop_length = 512

    # Calcular la matriz del espectrograma
    spec = np.abs(librosa.stft(y, n_fft=n_fft, hop_length=hop_length))**2

    # Calcular la frecuencia correspondiente a cada fila del espectrograma
    freqs = librosa.core.fft_frequencies(sr=sr, n_fft=n_fft)

    # Configurar la frecuencia máxima para considerar un componente como grave
    high_freq = 200

    # Buscar los índices correspondientes a las frecuencias graves
    low_idx = np.where(freqs <= high_freq)[0]

    # Calcular la magnitud del espectro para las frecuencias graves en cada cuadro de tiempo
    low_mag = np.mean(spec[low_idx], axis=0)

    # Convertir a decibelios
    low_db = librosa.amplitude_to_db(low_mag, ref=np.max)

    # Encontrar los cuadros de tiempo en los que la magnitud de las frecuencias graves supera un umbral
    low_threshold = np.max(low_db) - 30
    low_mask = low_db > low_threshold

    # Calcular los tiempos correspondientes a los cuadros de tiempo
    low_time = librosa.frames_to_time(np.arange(len(low_mask)), sr=sr, hop_length=hop_length)

    # Crear la matriz de tiempos y valores de los componentes graves
    graves_time_value = np.column_stack((low_time, low_db))

    return graves_time_value

def agudos(y, sr):

    # Calcular la transformada de Fourier de corto tiempo
    n_fft = 2048
    hop_length = 512
    stft = librosa.stft(y, n_fft=n_fft, hop_length=hop_length)

    # Calcular los valores de frecuencia correspondientes a cada fila de la matriz STFT
    freqs = librosa.fft_frequencies(sr=sr, n_fft=n_fft)

    # Definir las frecuencias de corte para los componentes agudos
    low_freq = 2000
    high_freq = freqs.max()

    # Crear una máscara para los componentes agudos
    mask = np.logical_and(freqs >= low_freq, freqs <= high_freq)

    # Aplicar la máscara a la matriz STFT para obtener los componentes agudos
    stft_high = stft[mask, :]

    # Calcular la energía de los componentes agudos en cada cuadro de tiempo
    energy_high = librosa.core.amplitude_to_db(np.abs(stft_high))

    # Calcular el umbral para detectar los componentes agudos
    threshold = np.median(energy_high)

    # Crear una matriz de tiempo y valor para los componentes agudos
    time_frames = librosa.frames_to_time(np.arange(energy_high.shape[1]), sr=sr, hop_length=hop_length)
    agudos_time_value = np.array([np.array([time_frames[i], np.max(energy_high[:, i])]) for i in range(energy_high.shape[1]) if np.max(energy_high[:, i]) > threshold])

    # Graficar los componentes agudos
    # plt.plot(agudos_time_value[:, 0], agudos_time_value[:, 1], 'bo', markersize=3)
    # plt.plot(graves_time_value[:, 0], graves_time_value[:, 1], 'xr', markersize=3)

    # # Configurar la gráfica
    # plt.xlabel('Tiempo (segundos)')
    # plt.ylabel('Valor de los componentes agudos (dB)')
    # plt.title('Componentes agudos y graves del audio')
    # plt.show()
    return agudos_time_value

'''
Dada una matriz con la primera columna de tiempos, devuelve una nueva matriz cada x seg
'''
def filtroSeg(matriz_original, seg):
    # Obtener el vector de tiempo correspondiente a la matriz original
    tiempo_original = matriz_original[:, 0]

    # Crear un nuevo vector de tiempo con los tiempos que quieres muestrear
    tiempo_nuevo = np.arange(tiempo_original[0], tiempo_original[-1], seg)

    # Buscar los índices correspondientes en la matriz original
    indices_nuevos = np.argmin(np.abs(tiempo_original[:, None] - tiempo_nuevo), axis=0)

    # Crear la nueva matriz con los valores correspondientes
    matriz_nueva = matriz_original[indices_nuevos, :]
    return matriz_nueva

def example(filename):
    y, sr = librosa.load(filename)

    # Set the hop length; at 22050 Hz, 512 samples ~= 23ms
    hop_length = 512

    # Separate harmonics and percussives into two waveforms
    y_harmonic, y_percussive = librosa.effects.hpss(y)

    # Beat track on the percussive signal
    tempo, beat_frames = librosa.beat.beat_track(y=y_percussive,
                                                sr=sr)

    # Compute MFCC features from the raw signal
    mfcc = librosa.feature.mfcc(y=y, sr=sr, hop_length=hop_length, n_mfcc=13)

    # And the first-order differences (delta features)
    mfcc_delta = librosa.feature.delta(mfcc)

    # Stack and synchronize between beat events
    # This time, we'll use the mean value (default) instead of median
    beat_mfcc_delta = librosa.util.sync(np.vstack([mfcc, mfcc_delta]),
                                        beat_frames)

    # Compute chroma features from the harmonic signal
    chromagram = librosa.feature.chroma_cqt(y=y_harmonic,
                                            sr=sr)

    # Aggregate chroma features between beat events
    # We'll use the median value of each feature between beat frames
    beat_chroma = librosa.util.sync(chromagram,
                                    beat_frames,
                                    aggregate=np.median)

    # Finally, stack all beat-synchronous features together
    beat_features = np.vstack([beat_chroma, beat_mfcc_delta])
    plt.plot(chromagram)
    plt.show()
    return beat_features

def percussive(filename):
    y, sr = librosa.load(filename)

    y_harm, y_perc = librosa.effects.hpss(y)
    

    # plt.subplot(3, 1, 3)

    #librosa.display.waveshow(y=y_harm, sr=sr, alpha=0.25)
    librosa.display.waveshow(y=y_perc, sr=sr, color='r', alpha=0.5)
    plt.title('Harmonic + Percussive')
    plt.show()

def beats(filename):
    y, sr = librosa.load(filename)
    onset_env = librosa.onset.onset_strength(y=y, sr=sr)
    pulse = librosa.beat.plp(onset_envelope=onset_env, sr=sr)

    tempo, beats = librosa.beat.beat_track(onset_envelope=onset_env)
    beats_plp = np.flatnonzero(librosa.util.localmax(pulse))
    fig, ax = plt.subplots(nrows=2, sharex=True, sharey=True)
    times = librosa.times_like(onset_env, sr=sr)

    # ax[0].plot(times, librosa.util.normalize(onset_env),
    #         label='Onset strength')
    # ax[0].vlines(times[beats], 0, 1, alpha=0.5, color='r',
    #         linestyle='--', label='Beats')
    # ax[0].legend()
    # ax[0].set(title='librosa.beat.beat_track')
    
    # times = librosa.times_like(pulse, sr=sr)
    # ax[1].plot(times, librosa.util.normalize(pulse),
    #         label='PLP')
    # ax[1].vlines(times[beats_plp], 0, 1, alpha=0.5, color='r',
    #         linestyle='--', label='PLP Beats')
    # ax[1].legend()
    # ax[1].set(title='librosa.beat.plp')
    # ax[1].xaxis.set_major_formatter(librosa.display.TimeFormatter())
    # plt.show()
    return times[beats_plp]

#--------------------FUNCIÓN PRINCIPAL QUE GENERA LOS .TXT-------------------------------
def features_to_txt(filename):
    # Carga la canción en un array de muestras
    samples, sr = load_Wave(filename) #mono=True

    # Sample rate
    np_sr = np.array([sr])

    # Beats
    bpm, beats = get_beats_in_timeline(samples, sr)

    # Spectral centroid
    sc = spectral_centroid_v1(samples, sr)
    scopt = matriz_coord_beats(sc, beats)

    # Rmse
    r = rmse(samples)
    ropt = matriz_coord_beats(r, beats)
    rnorm = normalize(ropt)

    # Graves
    g = graves(samples, sr)
    g = filtroSeg(g, 0.5)
    gTiempo = g[:, 0]
    gValorNorm = normalize(g[:, 1])

    # Agudos 
    a = agudos(samples, sr)
    a = filtroSeg(a, 0.5)
    aTiempo = a[:, 0]
    aValorNorm = normalize(a[:, 1])

    # Golpes de batería (onset)
    onset = onset_detection(samples, sr)

    name = os.path.splitext(filename)[0]
    np.savetxt(name + '_samples.txt', samples, fmt='%.3f')
    np.savetxt(name + '_sr.txt', np_sr, fmt='%.0f')
    np.savetxt(name + '_beats.txt', beats, fmt='%.3f')
    np.savetxt(name + '_rmse.txt', rnorm, fmt='%.3f')
    np.savetxt(name + '_scopt.txt', scopt, fmt='%.3f')
    np.savetxt(name + '_gravesTiempo.txt', gTiempo, fmt='%.3f')
    np.savetxt(name + '_gravesValorNorm.txt', gValorNorm, fmt='%.3f')
    np.savetxt(name + '_agudosTiempo.txt', aTiempo, fmt='%.3f')
    np.savetxt(name + '_agudosValorNorm.txt', aValorNorm, fmt='%.3f')
    np.savetxt(name + '_onsetDetection.txt', onset, fmt='%.3f')

    # Ruta de la carpeta de origen
    ruta_origen = './'

    # Ruta de la carpeta de destino
    ruta_destino = '../unity/Assets/Txt/'

    # Recorre los archivos de la carpeta de origen
    # for archivo in os.listdir(ruta_origen):
    #     # Verifica que el archivo sea un archivo de texto
    #     if archivo.endswith('.txt'):
    #         # Obtiene la ruta completa del archivo de origen
    #         ruta_archivo_origen = os.path.join(ruta_origen, archivo)
    #         # Obtiene la ruta completa del archivo de destino
    #         ruta_archivo_destino = os.path.join(ruta_destino, archivo)
    #         # Mueve el archivo de origen al archivo de destino
    #         os.replace(ruta_archivo_origen, ruta_archivo_destino)

#--------------------FUNCIONES QUE PUEDEN SER BORRADAS-------------------------------
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
        librosa.feature.zero_crossing_rate(
            signal)[0, 0],  # Numero de cruces por cero
        librosa.feature.spectral_centroid(
            signal)[0, 0],  # Centroide del espectro
    ]



def main():
    filename = 'Purpurina.wav'
    features_to_txt(filename)

main()