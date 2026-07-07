import { Status, Priority } from './Types';

export interface CreateTask {
  title: string;
  description: string;
  status: Status;
  priority: Priority;
  dueDate: string;
  assignedToId: string;
  projectId: string;
}
