"use client";
import { PropsWithChildren } from "react";
import styles from "./MasterLayout.module.scss";
import Container from "@mui/material/Container";
import MasterNav from "@adList/components/MasterNav/MasterNav";

export default function MasterLayout(props: PropsWithChildren<unknown>) {
    return (
        <div className={styles.siteWrapper}>
            <Container>
                <header className={styles.siteHeader} role="toolbar">
                    <MasterNav />
                </header>
                <main className={styles.main}>
                    {props.children}
                </main>
                <footer className={styles.siteFooter}>
                </footer>
            </Container>
        </div>
    );
}
