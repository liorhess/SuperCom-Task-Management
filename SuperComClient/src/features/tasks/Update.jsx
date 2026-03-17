import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { getTasksApiUrl, updateTask } from "./taskSlice";
import axios from "axios";
import {
  NAME_PATTERN,
  PHONE_PATTERN,
  PRIORITY_DEFAULT,
  PRIORITY_MAX,
  PRIORITY_MIN,
  TASK_TAG_OPTIONS,
  UI_MESSAGES,
} from "./taskConfig";

const Update = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const [formData, setFormData] = useState({
    title: "",
    description: "",
    dueDate: "",
    priority: PRIORITY_DEFAULT,
    userFullName: "",
    userTelephone: "",
    userEmail: "",
    tagNames: [],
  });

  useEffect(() => {
    const fetchCurrentTask = async () => {
      try {
        const response = await axios.get(`${getTasksApiUrl()}/${id}`);
        const taskData = response.data;
        if (taskData.dueDate) {
          taskData.dueDate = taskData.dueDate.split("T")[0];
        }
        taskData.tagNames = taskData.tagNames || [];
        setFormData(taskData);
      } catch {
        alert(UI_MESSAGES.taskLoadFailed);
        navigate("/");
      }
    };
    fetchCurrentTask();
  }, [id, navigate]);

  const handleChange = (e) => {
    const { name, value, type, selectedOptions } = e.target;

    if (name === "priority") {
      setFormData({
        ...formData,
        [name]: parseInt(value, 10) || PRIORITY_DEFAULT,
      });
    } else if (type === "select-multiple") {
      const values = Array.from(selectedOptions, (option) => option.value);
      setFormData({ ...formData, [name]: values });
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    dispatch(updateTask({ id, taskData: formData })).then((result) => {
      if (result.meta.requestStatus === "fulfilled") {
        alert(UI_MESSAGES.taskUpdated);
        navigate("/");
      }
    });
  };

  return (
    <div className="container py-3">
      <div className="row justify-content-center">
        <div className="col-md-10 col-lg-8">
          <div className="card shadow-sm border-0">
            <div className="card-body p-3">
              <h4 className="text-primary border-bottom pb-2 mb-3">
                Edit Task #{id}
              </h4>

              <form onSubmit={handleSubmit}>
                <div className="row g-2 mb-2">
                  <div className="col-md-6">
                    <label className="form-label small fw-bold mb-1">
                      Title
                    </label>
                    <input
                      name="title"
                      className="form-control form-control-sm"
                      value={formData.title}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="col-md-3">
                    <label className="form-label small fw-bold mb-1">
                      Due Date
                    </label>
                    <input
                      type="date"
                      name="dueDate"
                      className="form-control form-control-sm"
                      value={formData.dueDate}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="col-md-3">
                    <label className="form-label small fw-bold mb-1">
                      Priority (1-5)
                    </label>
                    <input
                      type="number"
                      name="priority"
                      className="form-control form-control-sm"
                      min={PRIORITY_MIN}
                      max={PRIORITY_MAX}
                      value={formData.priority}
                      onChange={handleChange}
                      required
                    />
                  </div>
                </div>

                <div className="mb-2">
                  <label className="form-label small fw-bold mb-1">
                    Description
                  </label>
                  <textarea
                    name="description"
                    className="form-control form-control-sm"
                    rows="2"
                    value={formData.description}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div className="bg-light p-2 rounded mb-2">
                  <div className="row g-2">
                    <div className="col-md-4">
                      <label className="form-label small mb-0">
                        Assignee Name
                      </label>
                      <input
                        name="userFullName"
                        className="form-control form-control-sm"
                        value={formData.userFullName}
                        onChange={handleChange}
                        required
                        pattern={NAME_PATTERN}
                        title="Full Name should only contain letters and spaces."
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label small mb-0">Telephone</label>
                      <input
                        type="tel"
                        name="userTelephone"
                        className="form-control form-control-sm"
                        value={formData.userTelephone}
                        onChange={handleChange}
                        required
                        pattern={PHONE_PATTERN}
                        title="Please enter a valid phone number (9-12 digits)"
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label small mb-0">Email</label>
                      <input
                        type="email"
                        name="userEmail"
                        className="form-control form-control-sm"
                        value={formData.userEmail}
                        onChange={handleChange}
                        required
                      />
                    </div>
                  </div>
                </div>

                <div className="mb-3">
                  <label className="form-label small fw-bold mb-1">
                    Tags (Hold Ctrl or Cmd to select multiple)
                  </label>
                  <select
                    name="tagNames"
                    multiple
                    className="form-select form-select-sm tag-select-box"
                    value={formData.tagNames}
                    onChange={handleChange}
                  >
                    {TASK_TAG_OPTIONS.map((tag) => (
                      <option key={tag} value={tag}>
                        {tag}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="d-flex justify-content-end gap-2 border-top pt-2">
                  <button
                    type="button"
                    className="btn btn-sm btn-outline-secondary px-4"
                    onClick={() => navigate("/")}
                  >
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-sm btn-primary px-4">
                    Update Task
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Update;
