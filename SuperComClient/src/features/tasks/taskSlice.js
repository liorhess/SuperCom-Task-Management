import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

export const API_URL = import.meta.env.VITE_API_URL?.trim();

export const getTasksApiUrl = () => {
  if (!API_URL) {
    throw new Error(
      "Missing VITE_API_URL. Set it in .env before running the app.",
    );
  }
  return API_URL;
};

export const fetchTasks = createAsyncThunk("tasks/fetchAll", async () => {
  const response = await axios.get(getTasksApiUrl());
  return response.data;
});

export const createTask = createAsyncThunk("tasks/create", async (taskData) => {
  const response = await axios.post(getTasksApiUrl(), taskData);
  return response.data;
});

export const updateTask = createAsyncThunk(
  "tasks/update",
  async ({ id, taskData }) => {
    const response = await axios.put(`${getTasksApiUrl()}/${id}`, taskData);
    return response.data;
  },
);

export const deleteTask = createAsyncThunk("tasks/delete", async (id) => {
  await axios.delete(`${getTasksApiUrl()}/${id}`);
  return id;
});

const taskSlice = createSlice({
  name: "tasks",
  initialState: { items: [], status: "idle", error: null },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchTasks.pending, (state) => {
        state.status = "loading";
        state.error = null;
      })
      .addCase(fetchTasks.fulfilled, (state, action) => {
        state.items = action.payload;
        state.status = "succeeded";
        state.error = null;
      })
      .addCase(fetchTasks.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      .addCase(createTask.pending, (state) => {
        state.status = "loading";
        state.error = null;
      })
      .addCase(createTask.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.error = null;
        state.items.push(action.payload);
      })
      .addCase(updateTask.pending, (state) => {
        state.status = "loading";
        state.error = null;
      })
      .addCase(updateTask.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.error = null;
        const index = state.items.findIndex((t) => t.id === action.payload.id);
        if (index !== -1) {
          state.items[index] = action.payload;
        }
      })
      .addCase(updateTask.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      .addCase(deleteTask.pending, (state) => {
        state.status = "loading";
        state.error = null;
      })
      .addCase(deleteTask.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.error = null;
        state.items = state.items.filter((task) => task.id !== action.payload);
      })
      .addCase(deleteTask.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      .addCase(createTask.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      });
  },
});

export default taskSlice.reducer;
