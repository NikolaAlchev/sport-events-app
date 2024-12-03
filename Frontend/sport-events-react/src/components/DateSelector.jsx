import styles from "../css/Matches.module.css";

function DateSelector({ date, dateFunc }) {

    const handleDateChange = (event) => {
        dateFunc(event.target.value);
    };

    return (
        <div className={styles.CalendarContainer}>
            <div className={styles.CalendarInnerContainer}>
                <p className={styles.CalendarText}>Select a Date</p>
                <input type="date" value={date} onChange={handleDateChange} style={{marginBottom: "20px"}}/>
                <hr />
            </div>
        </div>
    );
}

export default DateSelector;