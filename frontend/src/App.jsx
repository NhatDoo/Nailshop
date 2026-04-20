import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { logout } from './store/slices/authSlice';

// Pages
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import ForgotPassword from './pages/ForgotPassword';
import Booking from './pages/Booking';
import AdminServices from './pages/admin/AdminServices';
import AdminNailDesign from './pages/admin/AdminNailDesign';
import MyBookings from './pages/MyBookings';
import AdminTodo from './pages/admin/AdminTodo';

function App() {
  const { isAuthenticated, user } = useSelector((state) => state.auth);
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logout());
  };

  return (
    <BrowserRouter>
      {/* Removed static template loader to prevent blocking */}

      {/* header */}
      <header>
        <div className="header">
          <div className="container-fluid">
            <div className="row">
              <div className="col-xl-3 col-lg-3 col-md-3 col-sm-3 col logo_section">
                <div className="full">
                  <div className="center-desk">
                    <div className="logo">
                      <Link to="/"><img src="/images/logo.png" alt="#" /></Link>
                    </div>
                  </div>
                </div>
              </div>
              <div className="col-xl-7 col-lg-7 col-md-9 col-sm-9">
                <nav className="navigation navbar navbar-expand-md navbar-dark ">
                  <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample04" aria-controls="navbarsExample04" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                  </button>
                  <div className="collapse navbar-collapse" id="navbarsExample04">
                    <ul className="navbar-nav mr-auto">
                      <li className="nav-item">
                        <Link className="nav-link" to="/">Home</Link>
                      </li>

                      {isAuthenticated ? (
                        <>
                          {user?.role === 'Admin' ? (
                            /* ── ADMIN MENU ── */
                            <>
                              <li className="nav-item">
                                <Link className="nav-link" to="/admin/services">Quản lý DV</Link>
                              </li>
                              <li className="nav-item">
                                <Link className="nav-link" to="/admin/naildesign">Nail Design</Link>
                              </li>
                              <li className="nav-item">
                                <Link className="nav-link" style={{ color: '#00bcd4' }} to="/admin/todo">Lịch làm việc</Link>
                              </li>
                            </>
                          ) : (
                            /* ── CUSTOMER MENU ── */
                            <>
                              <li className="nav-item">
                                <Link className="nav-link" to="/booking">Booking</Link>
                              </li>
                              <li className="nav-item">
                                <Link className="nav-link" style={{ color: '#ffb3c6' }} to="/my-bookings">Lịch của tôi</Link>
                              </li>
                            </>
                          )}

                          {/* Shared: email + Logout */}
                          <li className="nav-item">
                            <span className="nav-link" style={{ color: '#ffc107', cursor: 'default' }}>
                              {user?.email || 'User'}
                            </span>
                          </li>
                          <li className="nav-item">
                            <button
                              onClick={handleLogout}
                              className="nav-link"
                              style={{ background: 'none', border: 'none', cursor: 'pointer', outline: 'none', color: '#ff7043' }}
                            >
                              Logout
                            </button>
                          </li>
                        </>
                      ) : (
                        /* ── GUEST MENU ── */
                        <>
                          <li className="nav-item">
                            <Link className="nav-link" to="/login">Sign In</Link>
                          </li>
                          <li className="nav-item">
                            <Link className="nav-link" to="/register">Register</Link>
                          </li>
                        </>
                      )}
                    </ul>
                  </div>
                </nav>
              </div>
              <div className="col-md-2">
                <ul className="social_icon">
                  <li><a href="#"><i className="fa fa-facebook" aria-hidden="true"></i></a></li>
                  <li><a href="#"><i className="fa fa-twitter" aria-hidden="true"></i></a></li>
                  <li><a href="#"><i className="fa fa-linkedin" aria-hidden="true"></i></a></li>
                  <li><a href="#"><i className="fa fa-instagram" aria-hidden="true"></i></a></li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </header>

      {/* Dynamic Content (Home, Login, Todo, etc.) */}
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/booking" element={<Booking />} />
        <Route path="/my-bookings" element={<MyBookings />} />
        <Route path="/admin/services" element={<AdminServices />} />
        <Route path="/admin/naildesign" element={<AdminNailDesign />} />
        <Route path="/admin/todo" element={<AdminTodo />} />
      </Routes>

      {/* footer */}
      <footer>
        <div className="footer">
          <div className="container">
            <div className="row">
              <div className="col-md-12">
                <ul className="conta">
                  <li><i className="fa fa-map-marker" aria-hidden="true"></i> Passages of Lorem Ipsum available</li>
                  <li><i className="fa fa-phone" aria-hidden="true"></i> Call : +012334567890</li>
                  <li> <i className="fa fa-envelope" aria-hidden="true"></i><a href="#"> demo@gmail.com</a></li>
                </ul>
              </div>
            </div>
          </div>
          <div className="copyright">
            <div className="container">
              <div className="row">
                <div className="col-md-10 offset-md-1">
                  <p>© 2026 All Rights Reserved. Nailshop Application </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </footer>
    </BrowserRouter>
  );
}

export default App;
