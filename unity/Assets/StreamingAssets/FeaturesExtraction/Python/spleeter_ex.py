import sys
import subprocess
import os

# input_file: ruta donde se encuentra la canción
# output_folder: ruta donde se dejan las pistas generadas
def main(input_file, output_folder):
    # Comando para llamar a spleeter
    command = [ 'cmd', '/c', 'spleeter', 'separate', '-p', 'spleeter:5stems',  '-o', output_folder, '-f', '{filename}_{instrument}.{codec}', input_file]
    subprocess.call(command)


main(sys.argv[1], sys.argv[2])
# main('200-BPM.wav', './')