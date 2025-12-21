import { useCallback, useEffect, useState, type TouchEvent } from "react";
import styles from "./ImageCarousel.module.css";

const minSwipeDistance = 50;
export function ImageCarousel({ images }: { images: string[] }) {
    const [currentImage, setCurrentImage] = useState<number>(0);

    const [touchStart, setTouchStart] = useState<number | null>(null);
    const [touchEnd, setTouchEnd] = useState<number | null>(null);

    const prevImage = useCallback(() => {
        setCurrentImage((prev) => (prev === 0 ? images.length - 1 : prev - 1));
    }, [images.length]);

    const nextImage = useCallback(() => {
        setCurrentImage((prev) => ((prev + 1) % images.length));
    }, [images.length]);

    const onKeyDown = useCallback((e: KeyboardEvent) => {
        if (e.key === 'ArrowLeft') {
            prevImage();
        } else if (e.key === 'ArrowRight') {
            nextImage();
        }
    }, [prevImage, nextImage]);

    useEffect(() => {
        document.addEventListener('keydown', onKeyDown);
        return () => document.removeEventListener('keydown', onKeyDown);
    }, [onKeyDown]);

    const onTouchStart = (e: TouchEvent<HTMLDivElement>) => {
        setTouchEnd(null);
        setTouchStart(e.targetTouches[0].clientX);
    };

    const onTouchMove = (e: TouchEvent<HTMLDivElement>) => {
        setTouchEnd(e.targetTouches[0].clientX);
    };

    const onTouchEnd = () => {
        if (!touchStart || !touchEnd) return
        const distance = touchStart - touchEnd
        const isLeftSwipe = distance > minSwipeDistance
        const isRightSwipe = distance < -minSwipeDistance
        if (isLeftSwipe) {
            nextImage();
        } else if (isRightSwipe) {
            prevImage();
        }
    }

    return (
        <div className={styles.container}>
            <div className={styles.wrapper}
                 onTouchStart={onTouchStart}
                 onTouchMove={onTouchMove}
                 onTouchEnd={onTouchEnd}>
                {images.map((image, index) => (
                    <img key={index}
                         src={image}
                         alt="&#10006;"
                         className={`${styles.image} ${currentImage === index ? '' : styles.hidden}`}/>
                ))}
            </div>

            <div className={styles.indicators}>
                {images.map((_, index) => (
                    <div
                        key={index}
                        className={`${styles.dot} ${currentImage === index ? styles.activeDot : ''}`}
                        onClick={() => setCurrentImage(index)}/>
                ))}
            </div>
        </div>
    );
}