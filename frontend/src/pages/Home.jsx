import React from 'react';
import { motion } from 'framer-motion';

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
    return (
        <motion.div initial="hidden" animate="visible" variants={containerVariants}>

            {/* banner */}
            <motion.section
                variants={{ hidden: { opacity: 0 }, visible: { opacity: 1, transition: { duration: 0.8 } } }}
                className="banner_main"
            >
                <div id="myCarousel" className="carousel slide banner1" data-ride="carousel">
                    <ol className="carousel-indicators">
                        <li data-target="#myCarousel" data-slide-to="0" className="active"></li>
                        <li data-target="#myCarousel" data-slide-to="1"></li>
                        <li data-target="#myCarousel" data-slide-to="2"></li>
                    </ol>
                    <div className="carousel-inner">
                        <div className="carousel-item active">
                            <div className="container-fluid">
                                <div className="carousel-caption relative">
                                    <div className="row d_flex">
                                        <motion.div
                                            className="col-md-6"
                                            initial={{ opacity: 0, x: -60 }}
                                            animate={{ opacity: 1, x: 0 }}
                                            transition={{ duration: 0.9, delay: 0.2 }}
                                        >
                                            <img className="bann_img" src="/images/banner_ing.jpg" alt="#" />
                                        </motion.div>
                                        <motion.div
                                            className="col-md-6"
                                            initial={{ opacity: 0, x: 60 }}
                                            animate={{ opacity: 1, x: 0 }}
                                            transition={{ duration: 0.9, delay: 0.4 }}
                                        >
                                            <span>01/03</span>
                                            <h1>Milina nail Salon Creating Beauty</h1>
                                            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempo</p>
                                            <motion.a
                                                className="get_btn"
                                                href="Javascript:void(0)"
                                                role="button"
                                                whileHover={{ scale: 1.06 }}
                                                whileTap={{ scale: 0.96 }}
                                            >
                                                Get Appointment
                                            </motion.a>
                                        </motion.div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <a className="carousel-control-prev" href="#myCarousel" role="button" data-slide="prev">
                        <i className="fa fa-long-arrow-left" aria-hidden="true"></i>
                        <span className="sr-only">Previous</span>
                    </a>
                    <a className="carousel-control-next" href="#myCarousel" role="button" data-slide="next">
                        <i className="fa fa-long-arrow-right" aria-hidden="true"></i>
                        <span className="sr-only">Next</span>
                    </a>
                </div>
            </motion.section>

            {/* what we do */}
            <motion.div variants={itemVariants} className="we_do slin">
                <div className="container">
                    <div className="row">
                        <div className="col-md-12">
                            <div className="titlepage">
                                <h2>What We Do</h2>
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
                                            <p>Lorem ipsum dolor sit amet, consectetur </p>
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
                                <h2>About Us</h2>
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
                                <motion.a
                                    className="read_more"
                                    href="Javascript:void(0)"
                                    whileHover={{ scale: 1.05 }}
                                >
                                    Read More
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
