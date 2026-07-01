import { Project } from './Project';
import { ProjectMember } from './ProjectMember';
import { Task } from './Task';

export interface ProjectDetail extends Project {
  tasks: Task[];
  members: ProjectMember[];
}
