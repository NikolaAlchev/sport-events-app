import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleChevronLeft } from '@fortawesome/free-solid-svg-icons';
import styles from "../css/BackButton.module.css"

function BackButton() {

    const navigate = useNavigate();

    const handleBack = () => {
        navigate(-1);
    };

    return (
        <div onClick={handleBack} className={styles.IconContainer}>
            <FontAwesomeIcon icon={faCircleChevronLeft} />
        </div>
    );
};

export default BackButton;
