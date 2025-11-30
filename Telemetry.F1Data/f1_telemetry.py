import fastf1
import json
import sys
import time

def load_session(year, gp_name, session_type="R"):
    fastf1.Cache.enable_cache("fastf1_cache")
    session = fastf1.get_session(year, gp_name, session_type)
    session.load()
    return session

def main():
    if len(sys.argv) < 4:
        print("Usage: python f1_telemetry.py <year> <gp_name> <driver>")
        return

    year = int(sys.argv[1])
    gp = sys.argv[2]
    driver = sys.argv[3]

    session = load_session(year, gp)
    laps = session.laps.pick_driver(driver)

    for _, lap in laps.iterlaps():
        lap_telemetry = lap.get_telemetry()

    for _, row in lap_telemetry.iterrows():
        data = {
            "Timestamp": row["Date"].isoformat(),
            "SpeedKmh": float(row.get("Speed", 0)),
            "Rpm": int(row.get("RPM", 0)),
            "Gear": int(row.get("nGear", 0)),
            "Throttle": float(row.get("Throttle", 0)) / 100,
            "Brake": float(row.get("Brake", 0)),
            "TireTempFL": 0,
            "TireTempFR": 0,
            "TireTempRL": 0,
            "TireTempRR": 0,
            "FuelRemainingKg": 0,
            "DrsActive": bool(row.get("DRS", 0)),
            "Lap": int(row.get("Lap", 0))
        }

        print(json.dumps(data), flush=True)
        time.sleep(0.05)


if __name__ == "__main__":
    main()
