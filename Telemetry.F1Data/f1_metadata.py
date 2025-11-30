import fastf1
import sys

fastf1.Cache.enable_cache("fastf1_cache")

def list_gps(year):
    schedule = fastf1.get_event_schedule(year)
    for _, row in schedule.iterrows():
        print(row.EventName)

def list_drivers(year, gp):
    session = fastf1.get_session(year, gp, "R")
    session.load()
    drivers = session.laps['Driver'].unique()
    for code in drivers:
        print(code)

def main():
    if len(sys.argv) < 3:
        print("Usage:")
        print("  python f1_metadata.py gp-list <year>")
        print("  python f1_metadata.py driver-list <year> <gp>")
        return

    cmd = sys.argv[1]

    if cmd == "gp-list":
        year = int(sys.argv[2])
        list_gps(year)

    elif cmd == "driver-list":
        year = int(sys.argv[2])
        gp = sys.argv[3]
        list_drivers(year, gp)

if __name__ == "__main__":
    main()
