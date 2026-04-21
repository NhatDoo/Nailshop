import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Link } from 'react-router-dom';
import axiosClient from '../utils/axiosClient';

const containerVariants = {
    hidden: { opacity: 0 },
    visible: {
        opacity: 1,
        transition: { staggerChildren: 0.2, delayChildren: 0.1 }
    }
};

const itemVariants = {
    hidden: { opacity: 0, y: 40 },
    visible: { opacity: 1, y: 0, transition: { duration: 0.7, type: 'spring', stiffness: 80 } }
};

const services = [
    { icon: 'weicon1.png', name: 'Nail Art' },
    { icon: 'weicon2.png', name: 'Manicure' },
    { icon: 'weicon3.png', name: 'Pedicure' },
    { icon: 'weicon4.png', name: 'Paraffin wax' },
];

const Home = () => {
    const [designs, setDesigns] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);

    useEffect(() => {
        axiosClient.get('/naildesign')
            .then(res => setDesigns(res.data))
            .catch(err => console.error("Error fetching designs:", err));
    }, []);

    useEffect(() => {
        if (designs.length <= 1) return;
        const timer = setInterval(() => {
            handleNext();
        }, 5000);
        return () => clearInterval(timer);
    }, [designs.length, currentIndex]);

    const handleNext = () => {
        setCurrentIndex((prev) => (prev + 1) % designs.length);
    };

    const handlePrev = () => {
        setCurrentIndex((prev) => (prev - 1 + designs.length) % designs.length);
    };

    // Current display data
    const hasDesigns = designs.length > 0;
    const currentDesign = hasDesigns ? designs[currentIndex] : null;

    return (
        <motion.div initial="hidden" animate="visible" variants={containerVariants}>

            {/* banner section */}
            <motion.section
                variants={{ hidden: { opacity: 0 }, visible: { opacity: 1, transition: { duration: 0.8 } } }}
                className="banner_main"
            >
                <div id="myCarousel" className="carousel slide banner1" data-ride="carousel">

                    {/* Indicators */}
                    {hasDesigns && (
                        <ol className="carousel-indicators">
                            {designs.map((_, i) => (
                                <li
                                    key={i}
                                    onClick={() => setCurrentIndex(i)}
                                    className={currentIndex === i ? 'active' : ''}
                                    style={{ cursor: 'pointer' }}
                                />
                            ))}
                        </ol>
                    )}

                    <div className="carousel-inner">
                        <div className="carousel-item active">
                            <div className="container-fluid">
                                <div className="carousel-caption relative">
                                    <div className="row d_flex">

                                        {/* Image Box */}
                                        <div className="col-md-6">
                                            <AnimatePresence mode='wait'>
                                                <motion.div
                                                    key={currentIndex}
                                                    initial={{ opacity: 0, x: -60 }}
                                                    animate={{ opacity: 1, x: 0 }}
                                                    exit={{ opacity: 0, x: 60 }}
                                                    transition={{ duration: 0.6 }}
                                                >
                                                    <img
                                                        className="bann_img"
                                                        src={hasDesigns ? currentDesign.imageUrl : "/images/banner_ing.jpg"}
                                                        alt="#"
                                                        style={{ borderRadius: hasDesigns ? '20px' : '0', boxShadow: hasDesigns ? '0 10px 30px rgba(0,0,0,0.15)' : 'none' }}
                                                    />
                                                </motion.div>
                                            </AnimatePresence>
                                        </div>

                                        {/* Text Box */}
                                        <div className="col-md-6">
                                            <AnimatePresence mode='wait'>
                                                <motion.div
                                                    key={currentIndex}
                                                    initial={{ opacity: 0, x: 60 }}
                                                    animate={{ opacity: 1, x: 0 }}
                                                    exit={{ opacity: 0, x: -60 }}
                                                    transition={{ duration: 0.6 }}
                                                >
                                                    <span>{hasDesigns ? `${currentIndex + 1}/${designs.length}` : "00/00"}</span>
                                                    <h1>{hasDesigns ? currentDesign.name : "Milina nail Salon Creating Beauty"}</h1>
                                                    <p>{hasDesigns ? (currentDesign.description || "Mẫu nail cao cấp được thiết kế bởi chuyên gia.") : "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempo"}</p>

                                                    <motion.div whileHover={{ scale: 1.06 }} whileTap={{ scale: 0.96 }}>
                                                        <Link className="get_btn" to="/booking">Get Appointment</Link>
                                                    </motion.div>
                                                </motion.div>
                                            </AnimatePresence>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Controls */}
                    {hasDesigns && (
                        <>
                            <a className="carousel-control-prev" href="javascript:void(0)" role="button" onClick={handlePrev}>
                                <i className="fa fa-long-arrow-left" aria-hidden="true"></i>
                                <span className="sr-only">Previous</span>
                            </a>
                            <a className="carousel-control-next" href="javascript:void(0)" role="button" onClick={handleNext}>
                                <i className="fa fa-long-arrow-right" aria-hidden="true"></i>
                                <span className="sr-only">Next</span>
                            </a>
                        </>
                    )}
                </div>
            </motion.section>

            {/* what we do */}
            <motion.div variants={itemVariants} className="we_do slin">
                <div className="container">
                    <div className="row">
                        <div className="col-md-12">
                            <div className="titlepage text-center">
                                <h2>Dịch Vụ Của Chúng Tôi</h2>
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-lg-10 offset-lg-1 col-md-12">
                            <div className="row">
                                {services.map((s, idx) => (
                                    <motion.div
                                        key={idx}
                                        className="col-md-3 col-sm-6"
                                        variants={itemVariants}
                                        whileHover={{ scale: 1.07, y: -6 }}
                                        whileTap={{ scale: 0.96 }}
                                        transition={{ type: 'spring', stiffness: 200 }}
                                    >
                                        <div className="we_box">
                                            <i><img src={`/images/${s.icon}`} alt="#" /></i>
                                            <h3>{s.name}</h3>
                                            <p>Chất lượng hàng đầu cho bộ móng của bạn.</p>
                                        </div>
                                    </motion.div>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            </motion.div>

            {/* about */}
            <motion.div variants={itemVariants} className="about slin2">
                <div className="container">
                    <div className="row d_flex">
                        <motion.div
                            className="col-md-6"
                            initial={{ opacity: 0, x: -40 }}
                            whileInView={{ opacity: 1, x: 0 }}
                            viewport={{ once: true }}
                            transition={{ duration: 0.7 }}
                        >
                            <div className="titlepage">
                                <h2>Về Salon Chúng Tôi</h2>
                                <p>Với đội ngũ kỹ thuật viên tay nghề cao và trang thiết bị hiện đại, Milina Nail Salon cam kết mang lại trải nghiệm làm đẹp tuyệt vời nhất cho quý khách.</p>
                                <motion.a
                                    className="read_more"
                                    href="javascript:void(0)"
                                    whileHover={{ scale: 1.05 }}
                                >
                                    Xem Thêm
                                </motion.a>
                            </div>
                        </motion.div>
                        <motion.div
                            className="col-md-6"
                            initial={{ opacity: 0, x: 40 }}
                            whileInView={{ opacity: 1, x: 0 }}
                            viewport={{ once: true }}
                            transition={{ duration: 0.7, delay: 0.2 }}
                        >
                            <div className="about_img">
                                <motion.figure whileHover={{ rotate: 2, scale: 1.03 }} transition={{ type: 'spring' }}>
                                    <img src="/images/about.png" alt="#" />
                                </motion.figure>
                            </div>
                        </motion.div>
                    </div>
                </div>
            </motion.div>

        </motion.div>
    );
};

export default Home;
