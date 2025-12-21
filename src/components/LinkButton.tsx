import styles from "./LinkButton.module.css";

interface LinkButtonProps {
    topText: string;
    bottomText: string;
    path: string;
    href: string;
}

export function LinkButton(props: LinkButtonProps) {
    return (
        <a className={styles.downloadButton}
           href={props.href}
           target="_blank"
           rel="noopener noreferrer">
            <svg viewBox="0 0 24 24"
                 width="24"
                 height="24"
                 fill="currentColor">
                <path d={props.path}/>
            </svg>
            <div className={styles.buttonText}>
                <span>{props.topText}</span>
                <strong>{props.bottomText}</strong>
            </div>
        </a>
    );
}