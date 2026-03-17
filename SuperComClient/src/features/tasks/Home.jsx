import React, { useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { fetchTasks, deleteTask } from "./taskSlice";
import { UI_MESSAGES } from "./taskConfig";

const formatDueDate = (value) => {
  if (!value) {
    return "-";
  }

  const parsedDate = new Date(value);
  return Number.isNaN(parsedDate.getTime())
    ? "-"
    : parsedDate.toLocaleDateString();
};

const Home = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { items, status, error } = useSelector((state) => state.tasks);

  useEffect(() => {
    if (status === "idle") {
      dispatch(fetchTasks());
    }
  }, [status, dispatch]);

  const handleDelete = (id) => {
    if (window.confirm(UI_MESSAGES.deleteConfirm)) {
      dispatch(deleteTask(id));
    }
  };

  return (
    <div className="container-fluid py-4 px-5">
      {" "}
      {/* Using container-fluid for more width */}
      <div className="d-flex justify-content-between align-items-center mb-4 border-bottom pb-2">
        <h2 className="text-secondary fw-bold mb-0">Task Dashboard</h2>
      </div>
      {status === "loading" && (
        <div className="text-center my-5">
          <div className="spinner-border text-primary" role="status"></div>
          <p className="mt-2">Loading tasks...</p>
        </div>
      )}
      {status === "failed" && (
        <div className="alert alert-danger">Error: {error}</div>
      )}
      {status === "succeeded" && (
        <div className="card shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover table-sm align-middle mb-0">
              {" "}
              {/* added table-sm */}
              <thead className="table-light">
                <tr className="small text-uppercase text-muted">
                  <th className="ps-3">ID</th>
                  <th>Title</th>
                  <th>Description</th>
                  <th>Due Date</th>
                  <th>Priority</th>
                  <th>Assignee</th>
                  <th>Phone</th>
                  <th>Email</th>
                  <th>Tags</th>
                  <th className="text-end pe-3">Actions</th>
                </tr>
              </thead>
              <tbody className="small">
                {items.length === 0 ? (
                  <tr>
                    <td colSpan="10" className="text-center py-5">
                      No tasks found.
                    </td>
                  </tr>
                ) : (
                  items.map((task) => (
                    <tr key={task.id}>
                      <td className="ps-3 text-muted">{task.id}</td>
                      <td className="fw-bold text-primary">{task.title}</td>
                      <td
                        className="text-truncate"
                        style={{ maxWidth: "150px" }}
                      >
                        {task.description}
                      </td>
                      <td>{formatDueDate(task.dueDate)}</td>
                      <td>
                        <span
                          className={`badge ${task.priority > 3 ? "bg-danger" : "bg-primary"}`}
                        >
                          {task.priority}
                        </span>
                      </td>
                      {/* NEW COLUMNS START HERE */}
                      <td>{task.userFullName}</td>
                      <td>{task.userTelephone}</td>
                      <td className="small text-muted">{task.userEmail}</td>
                      {/* NEW COLUMNS END HERE */}
                      <td>
                        {task.tagNames?.map((tag) => (
                          <span
                            key={tag}
                            className="badge bg-light text-dark border me-1"
                          >
                            {tag}
                          </span>
                        ))}
                      </td>
                      <td className="text-end pe-3">
                        <div className="btn-group btn-group-sm">
                          <button
                            className="btn btn-outline-primary"
                            onClick={() => navigate(`/edit/${task.id}`)}
                          >
                            Edit
                          </button>
                          <button
                            className="btn btn-outline-danger"
                            onClick={() => handleDelete(task.id)}
                          >
                            Delete
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};

export default Home;
