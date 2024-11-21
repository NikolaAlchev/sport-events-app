import styles from "../css/Matches.module.css";

function DateSelector({ date }) {
    return (
        <div className={styles.CalendarContainer}>
            <div className={styles.CalendarInnerContainer}>
                <p className={styles.CalendarText}>Select a Date</p>
                {/* treba Da se sredi kalendarot */}
                {/* <input type="date" value={date} onChange={handleDateChange}/> */}
                <input type="date" value={date}/>
                <hr />
            </div>
        </div>
    );
}

export default DateSelector;