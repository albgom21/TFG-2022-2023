import sys
import subprocess
import os

# input_file: ruta donde se encuentra la canción
# output_folder: ruta donde se dejan las pistas generadas
def main(input_file, output_folder):
    # Comando para llamar a spleeter

    # ruta_actual1 = os.getcwd()
    # print("0", ruta_actual1)
    # os.chdir(ruta_actual1 + "\\Assets\\FeaturesExtraction\\")
    # ruta_actual = os.getcwd()
    # print("1", ruta_actual)

    command = [ 'cmd', '/c', 'spleeter', 'separate', '-p', 'spleeter:2stems',  '-o', output_folder, '-f', '{filename}_{instrument}.{codec}', input_file]
    subprocess.call(command)


main(sys.argv[1], sys.argv[2])
# main('200-BPM.wav', './')