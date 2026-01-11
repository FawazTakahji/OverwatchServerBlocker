import styles from "./ContactLink.module.css";

interface ContactLinkProps {
    link: string;
    text: string;
    icon: string;
}

export function ContactLink(props: ContactLinkProps) {
    return (
        <a className={styles.container}
           href={props.link}
           target="_blank"
           rel="noopener noreferrer">
            <svg className={styles.icon}
                 viewBox="0 0 24 24">
                <path d={props.icon} />
            </svg>
            <span>{props.text}</span>
        </a>
    );
}