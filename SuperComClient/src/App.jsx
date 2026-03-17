import React from "react";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import "./App.css";

// We only need the big 3!
import Home from "./features/tasks/Home";
import Create from "./features/tasks/Create";
import Update from "./features/tasks/Update";

function App() {
  return (
    <BrowserRouter>
      <div className="container mt-4">
        {/* Simple Navigation Header */}
        <header className="app-header">
          <h1>SuperCom Task Manager</h1>
          <nav>
            {/* We only need links for Home and Create. 
                Users get to 'Update' by clicking a button inside Home. */}
            <Link to="/" className="nav-link-custom">
              Home (Task List)
            </Link>
            <Link to="/create" className="nav-link-custom">
              + Create Task
            </Link>
          </nav>
        </header>

        {/* The Routes */}
        <Routes>
          {/* '/' is the default route */}
          <Route path="/" element={<Home />} />

          <Route path="/create" element={<Create />} />

          {/* Note the /:id here! This passes the ID to the Update component */}
          <Route path="/edit/:id" element={<Update />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
