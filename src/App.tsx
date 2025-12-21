import styles from './App.module.css'
import { githubIcon, linuxIcon, windowsIcon } from "./svg.ts";
import windowsRelease from "../public/updates/windows.json";
import { LinkButton } from "./components/LinkButton.tsx";
import { ImageCarousel } from "./components/ImageCarousel.tsx";

function App() {
    return (
        <div>
            <header className={styles.header}>
                <a className={styles.headerBackLink}
                   href='/'>
                    Home
                </a>
                <h1 className={styles.headerTitle}>
                    Overwatch Server Blocker
                </h1>
            </header>

            <div className={styles.container}>
                <section className={styles.section}>
                    <ImageCarousel images={[
                        "screenshots/01.png",
                        "screenshots/02.png",
                        "screenshots/03.png",
                        "screenshots/04.png"
                    ]} />
                </section>

                <section className={styles.section}>
                    <h2 className={styles.sectionTitle}>
                        Links
                    </h2>
                    <div className={styles.linksWrapper}>
                        <LinkButton topText="Download for"
                                    bottomText={"Windows"}
                                    path={windowsIcon}
                                    href={windowsRelease.directDownload} />

                        <div className={styles.unclickable}>
                            <LinkButton topText={"Coming Soon"}
                                        bottomText={"Linux"}
                                        path={linuxIcon}
                                        href={"/"} />
                        </div>

                        <LinkButton topText={"Source Code"}
                                    bottomText={"Github"}
                                    path={githubIcon}
                                    href={"https://github.com/FawazTakahji/OverwatchServerBlocker"} />
                    </div>
                </section>

                <section>
                    <h2 className={styles.sectionTitle}>
                        How does it work?
                    </h2>
                    <p className={styles.sectionText}>
                        This application works by retrieving the latest ip ranges from the server providers, insuring that the ranges will always be up to date.
                        <br/>
                        <br/>
                        The application will then block the servers from being accessed by the game client over specific UDP ports, ensuring that the game will not connect to any blocked lobby servers while also avoiding interference with other network traffic like authorization.
                    </p>
                </section>

                <section>
                    <h2 className={styles.sectionTitle}>
                        Quick Start
                    </h2>
                    <p className={styles.sectionText}>
                        <p><strong>1.</strong> {"Ensure that you have the "}
                            <a href={"https://dotnet.microsoft.com/en-us/download/dotnet/10.0"}
                               target="_blank"
                               rel="noopener noreferrer">
                                .NET 10
                            </a>
                            {" runtime installed"}</p>

                        <p><strong>2. </strong>Run the application</p>

                        <p><strong>3. </strong>Select the regions you want to block</p>

                        <p><strong>4. </strong>Click "Apply"</p>

                        <p><strong>5. </strong>Launch Overwatch 2, you can close the application and the firewall rule will persist</p>
                    </p>
                </section>

                <section>
                    <h2 className={styles.sectionTitle}>
                        Safety
                    </h2>
                    <p className={styles.sectionText}>
                        This application does not modify any game files, and does not inject any code into the game process. It simply modifies the firewall rules to block the servers from being accessed.
                    </p>
                </section>
            </div>
        </div>
    )
}

export default App
