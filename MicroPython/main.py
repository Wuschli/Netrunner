import beacon
import scanner
import time

if __name__ == "__main__":
    beacon.advertise()
    scanner.scan()

    while (True):
        time.sleep(1)
