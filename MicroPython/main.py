import beacon
import scanner
import time
import machine

if __name__ == "__main__":
    print(machine.unique_id())
    beacon.advertise(machine.unique_id())
    # scanner.scan()

    while (True):
        time.sleep(1)
