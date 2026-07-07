import { Status, Priority } from './Types';

export interface Task {
  id: string;
  title: string;
  description: string;
  status: Status;
  priority: Priority;
  dueDate: Date;
  assignedToId: string;
  assignedToName: string;
  createdAt: Date;
  projectId: string;
}
